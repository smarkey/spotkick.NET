using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Spotkick.Services
{
    public static class CommonService
    {
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
        };

        private static async Task<T> Retry<T>(Func<T> func, int retryCount)
        {
            try
            {
                var result = await Task.Run(func);
                return result;
            }
            catch
            {
                if (retryCount == 0) throw;
            }

            return await Retry(func, --retryCount);
        }
    }
}
