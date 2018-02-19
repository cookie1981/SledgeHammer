using System;
using Newtonsoft.Json.Linq;
using ExpectedWipElements = profiling.RequestValidation.ExpectedWipElements;

namespace profiling.Storage
{
    public static class JObjectExtensions
    {
        private const string CreatedAtElementName = "CreatedAt";
        private const string ModifiedAtElementName = "ModifiedAt";

        public static void SetCreatedAt(this JObject jObject, DateTime datetime)
        {
            SetCreatedAt(jObject, (JToken) datetime);
        }

        public static void SetCreatedAt(this JObject jObject, JToken datetime)
        {
            if (jObject == null)
            {
                throw CreateNewArgumentException(nameof(jObject));
            }

            if (MetadataElementExists(jObject))
            {
                jObject[ExpectedWipElements.Metadata][CreatedAtElementName] = datetime;
            }
        }
        
        public static void SetModifiedAt(this JObject jObject, DateTime datetime)
        {
            if (jObject == null)
            {
                throw CreateNewArgumentException(nameof(jObject));
            }

            if (MetadataElementExists(jObject))
            {
                jObject[ExpectedWipElements.Metadata][ModifiedAtElementName] = datetime;
            }
        }

        public static JToken CreatedAt(this JObject jObject)
        {
            if (jObject == null)
            {
                throw CreateNewArgumentException(nameof(jObject));
            }

            if (MetadataElementExists(jObject))
            {
                return jObject[ExpectedWipElements.Metadata][CreatedAtElementName];
            }

            return null;
        }
        
        private static Exception CreateNewArgumentException(string paramName)
        {
            return new ArgumentException("JObject instance is null", paramName);
        }

        private static bool MetadataElementExists(JObject jObject)
        {
            return jObject[ExpectedWipElements.Metadata] != null;
        }
    }
}