using Spotkick.Services;
using Xunit;

namespace Spotkick.Test.Unit
{
    public class BandinstownServiceSpec
    {
        [Fact]
        public void UnsupportedCharactersAreReplacedWithEncodedCharacters()
        {
            var bandsintownService = new BandsintownService();

            const string unsupportedCharacters = @"?/*""";

            Assert.Equal("%253F%252F%252A%27C", bandsintownService.ReplaceUnsupportedChars(unsupportedCharacters));
        }
    }
}
