using System.Linq;

namespace Spotkick.Utils
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string str) => string
            .Concat(str
                .Replace(" ", "")
                .Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
            .ToLower();
    }
}