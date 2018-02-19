using Risk.Schema.Validator;

namespace profiling.RequestValidation
{
    public static class SchemaValidatorExtenstions
    {
        public static bool DoesNotContainSchema(this ISchemaValidator validator, string product)
        {
            return !validator.ContainsSchema(product);
        }
    }
}