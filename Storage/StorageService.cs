using System;
using System.Threading.Tasks;
using MicroMachines.Common.Interfaces;
using MicroMachines.Common.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using DataAccessException = profiling.Storage.Mongo.DataAccessException;
using IBsonIdGenerator = profiling.Bson.IBsonIdGenerator;
using IDateTimeProvider = profiling.DateTimeProvider.IDateTimeProvider;
using WipRisk = profiling.Models.WipRisk;

namespace profiling.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IDataRepository<WipRisk> _dataRepository;
        private readonly ITrackingInfo _trackingInfo;
        private readonly ILogger<StorageService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBsonIdGenerator _bsonIdProvider;

        public StorageService(IDataRepository<WipRisk> provider, ITrackingInfo trackingInfo, ILogger<StorageService> logger, IDateTimeProvider dateTimeProvider, IBsonIdGenerator bsonIdProvider)
        {
            _dataRepository = provider;
            _trackingInfo = trackingInfo;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _bsonIdProvider = bsonIdProvider;
        }
        
        public async Task<string> Save(ProductVersion productVersion, JObject wip)
        {
            if (productVersion.Equals(null))
            {
                {
                    throw new MissingMandatoryDataException($"{nameof(productVersion)} is missing"); throw new MissingMandatoryDataException($"{nameof(productVersion)} is missing");
                }
            }

            if (wip == null)
            {
                throw new MissingMandatoryDataException("Wip is null");
            }

            var wipId = _bsonIdProvider.GenerateId();
            var createdAt = _dateTimeProvider.Now;

            SetMetaData(wip, createdAt);

            var wipRisk = CreateWipRisk(productVersion, wip, wipId, createdAt);

            try
            {
                await _dataRepository.SaveAsync(wipRisk);
            }
            catch (Exception ex)
            {
                LogAndThrowDataAccessException(ex);
            }

            return wipId;
        }

        public async Task<JObject> Fetch(ProductVersion productVersion, string wipId)
        {
            var wipRisk = await FetchDocumentFromDataStore(productVersion, wipId);
            return wipRisk != null ? JObject.Parse(wipRisk.Payload.ToJson()) : null;
        }

        public async Task<JObject> Update(ProductVersion productVersion, JObject wip, string wipId)
        {
            var retrievedWipRisk = await FetchDocumentFromDataStore(productVersion, wipId);

            if (retrievedWipRisk == null)
            {
                return null;
            }

            var modifiedAt = _dateTimeProvider.Now;

            UpdateMetadata(wip, retrievedWipRisk, modifiedAt);

            var updatedWipRisk = CreateWipRisk(productVersion, wip, wipId, modifiedAt, retrievedWipRisk.WipVersion+1);

            var response = await _dataRepository.UpdateAsync(updatedWipRisk);

            return JObject.Parse(response.Payload.ToJson());
        }

        private static void SetMetaData(JObject wip, DateTime createdAt)
        {
            wip.SetModifiedAt(createdAt);
            wip.SetCreatedAt(createdAt);
        }

        private static void UpdateMetadata(JObject wip, WipRisk retrievedWipRisk, DateTime modifiedAt)
        {
            var existingPayload = JObject.Parse(retrievedWipRisk.Payload.ToJson());
            wip.SetCreatedAt(existingPayload.CreatedAt());
            wip.SetModifiedAt(modifiedAt);
        }

        private async Task<WipRisk> FetchDocumentFromDataStore(ProductVersion productVersion, string wipId)
        {
            if (productVersion.Equals(null))
            {
                throw new MissingMandatoryDataException($"{nameof(productVersion)} is missing");
            }

            ValidateParameter(wipId, nameof(wipId));

            try
            {
                //maybe use a filterclass to do ths rather than mongorepository
                var wipRisk = await _dataRepository.FetchAsync(wipId);

                if (wipRisk == null)
                {
                    return null;
                }

                if (!CheckStringsMatchNonCaseSensitive(productVersion.Product.ToString(), wipRisk.Product) ||
                    !CheckStringsMatchNonCaseSensitive(productVersion.Version.ToString(), wipRisk.Version))
                {
                    _logger.LogError($"Product/Version miss-match fetching wip: {wipId}");
                    return null;
                }

                ThrowIfNotMatchingSession(wipRisk);

                return wipRisk;
            }
            catch (SessionMismatchException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogAndThrowDataAccessException(ex);
            }

            return null;
        }

        private WipRisk CreateWipRisk(ProductVersion productVersion, JObject wip, string wipId, DateTime createdAt)
        {
            const int defaultWipVersion = 0;
            return CreateWipRisk(productVersion, wip, wipId, createdAt, defaultWipVersion);
        }

        private WipRisk CreateWipRisk(ProductVersion productVersion, JObject wip, string wipId, DateTime modifiedAt, int wipVersion)
        {
            var wipRisk = new WipRisk
            {
                Id = wipId,
                Product = productVersion.Product.ToString(),
                Version = productVersion.Version.ToString(),
                SessionId = _trackingInfo.SessionId.Value,
                VisitorId = _trackingInfo.VisitorId.Value,
                Payload = BsonDocument.Parse(wip.ToString()),
                ModifiedAt = modifiedAt,
                WipVersion = wipVersion
            };

            return wipRisk;
        }

        private void LogAndThrowDataAccessException(Exception ex)
        {
            const string message = "Error accessing data store";
            _logger.LogCritical(ex, message);
            throw new DataAccessException(message, ex);
        }

        private static void ValidateParameter(string parameterValue, string parameterName)
        {
            if (string.IsNullOrEmpty(parameterValue))
            {
                throw new MissingMandatoryDataException($"{parameterName} is missing");
            }
        }

        private static bool CheckStringsMatchNonCaseSensitive(string firstString, string secondString)
        {
            return string.Equals(firstString, secondString, StringComparison.CurrentCultureIgnoreCase);
        }

        private void ThrowIfNotMatchingSession(WipRisk result)
        {
            if (result.SessionId != _trackingInfo.SessionId || result.VisitorId != _trackingInfo.VisitorId)
            {
                _logger.LogWarning("SessionMismatch", $"Was expecting session id: '{result.SessionId}' and visitor id: '{result.VisitorId}'");

//                throw new SessionMismatchException();
            }
        }
    }
}