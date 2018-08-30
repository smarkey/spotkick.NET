using Newtonsoft.Json;
using Spotkick.Services;
using Xunit;

namespace Spotkick.Test.Unit
{
    public class TestDeserializedObject
    {
        public int UpperCamelCasedMember { get; set; }
    }

    public class CommonServiceSpec
    {
        [Fact]
        public void CommonSerializerSettingsShouldConvertSnakeCaseToUpperCamelCase()
        {
            var serializerSettings = CommonService.SerializerSettings;

            var deserializedObject = JsonConvert.DeserializeObject<TestDeserializedObject>("{upper_camel_cased_member:5}", serializerSettings);

            Assert.Equal(5, deserializedObject.UpperCamelCasedMember);
        }
    }
}
