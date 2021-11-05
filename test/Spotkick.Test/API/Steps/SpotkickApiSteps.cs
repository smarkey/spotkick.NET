using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NJsonSchema.Validation;
using Shouldly;
using Spotkick.Test.API.Schemas;
using TechTalk.SpecFlow;

namespace Spotkick.Test.API.Steps
{
    [Binding]
    public sealed class SpotkickApiSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly HttpClient _client;
        private readonly JsonSchemaValidator _validator;

        public SpotkickApiSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:6254"),
                DefaultRequestHeaders =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
            };
            _validator = new JsonSchemaValidator();
        }

        [Given(@"I have access to the Spotkick API")]
        public async Task GivenIHaveAccessToTheSpotkickApi()
        {
            (await _client.GetAsync("")).StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [When(@"I perform a '(.*)' on the '(.*)' endpoint")]
        public async Task WhenIPerformAOnTheEndpoint(string httpMethod, string endpoint)
        {
            await WhenIPerformAOnTheEndpoint(httpMethod, endpoint, null);
        }
        
        [When(@"I perform a '(.*)' on the '(.*)' endpoint with the payload '(.*)'")]
        public async Task WhenIPerformAOnTheEndpoint(string httpMethod, string endpoint, string payload)
        {
            var response = httpMethod switch
            {
                "GET" => await _client.GetAsync(endpoint),
                "POST" => await _client.PostAsync(endpoint, new StringContent(payload, System.Text.Encoding.UTF8, "application/json")),
                _ => throw new ArgumentException($"{httpMethod} is unsupported by this BDD step")
            };

            _scenarioContext.Add("StatusCode", response.StatusCode);
            _scenarioContext.Add("Body", response.Content);
        }

        [Then(@"I get a response of '(.*)'")]
        public void ThenIGetAResponseOf(HttpStatusCode statusCode)
        {
            _scenarioContext.Get<HttpStatusCode>("StatusCode").ShouldBe(statusCode);
        }

        [Then(@"the response body matches the json schema for a '(.*)'")]
        public async Task ThenTheResponseBodyMatchesTheJsonSchemaForA(string schema)
        {
            var body = await _scenarioContext.Get<HttpContent>("Body").ReadAsStringAsync();
            var schemaDefinition = await SchemaHandler.GetSchemaDefinition(schema);
            _validator.Validate(body, schemaDefinition).ShouldBeEmpty();
        }
    }
}