using System;
using System.Threading;
using System.Threading.Tasks;
using MicroMachines.Common.Interfaces;
using MicroMachines.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
//using RiskCapture.Storage.Mongo;

namespace profiling.HealthChecks
{
    public class MongoServiceHealthCheck<T> : IServiceHealthCheck
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoServiceHealthCheck(Storage.Mongo.IMongoCollectionProvider<T> mongoCollectionProvider)
        {
            _mongoDatabase = mongoCollectionProvider.Database;
        }

        public async Task<ServiceHealthCheckResult> PerformHealthCheckAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var isOk = false;

            try
            {
                var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument("ping", 1));
                await _mongoDatabase.RunCommandAsync(command, ReadPreference.Nearest, cancellationToken);

                isOk = true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                // ignored
            }

            return new ServiceHealthCheckResult
            {
                Url = null,
                IsOk = isOk,
                ServiceName = ServiceName
            };
        }

        private string ServiceName => "Mongo DB";
    }
}