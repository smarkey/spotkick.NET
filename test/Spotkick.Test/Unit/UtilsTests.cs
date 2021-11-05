using System.Threading.Tasks;
using Shouldly;
using Spotkick.Utils;
using Xunit;

namespace Spotkick.Test.Unit
{
    public class UtilsTests
    {
        [Theory]
        [InlineData("iAmCamelCased", "i_am_camel_cased")]
        [InlineData("IAmPascalCased", "i_am_pascal_cased")]
        [InlineData("I Am A Capitalised Sentence", "i_am_a_capitalised_sentence")]
        [InlineData("i_am_already_snake_cased", "i_am_already_snake_cased")]
        public async Task WhenIConvertTextToSnakeCase_ThenIShouldHaveSnakeCasedText(string input, string output)
        {
            // Arrange + Act
            var result = input.ToSnakeCase();

            // Assert
            result.ShouldBe(output);
        }

        [Fact]
        public async Task WhenIUseTheJsonSnakeCaseNamingPolicy_ThenIGetSnakeCasedText()
        {
            // Arrange
            var policy = new JsonSnakeCaseNamingPolicy();
            var textToConvert = "iAmCamelCased";

            // Act
            var result = policy.ConvertName(textToConvert);

            // Assert
            result.ShouldBe("i_am_camel_cased");
        }
    }
}