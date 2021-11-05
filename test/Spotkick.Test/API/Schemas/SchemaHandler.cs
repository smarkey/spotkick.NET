using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema;

namespace Spotkick.Test.API.Schemas
{
    public static class SchemaHandler
    {
        private static string OutputFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SchemaFolder => $"{OutputFolder}\\API\\Schemas";

        public static async Task<JsonSchema> GetSchemaDefinition(string schema) =>
            await JsonSchema.FromFileAsync($"{SchemaFolder}\\{schema}.json");
    }
}