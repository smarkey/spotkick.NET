using System.Linq;
using System.Text.Json;

namespace Spotkick.Utils
{
    public class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToSnakeCase();
        }
    }

    public static class StringExtensions
    {
        public static string ToSnakeCase(this string str) => string
            .Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
            .ToLower();
    }
}