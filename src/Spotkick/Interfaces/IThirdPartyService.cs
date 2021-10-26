using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Spotkick.Interfaces
{
    public interface IThirdPartyService
    {
        ILogger _logger { get; set; }
        HttpClient _client { get; set; }
        JsonSerializerOptions _serializerOptions { get; set; }
    }
}