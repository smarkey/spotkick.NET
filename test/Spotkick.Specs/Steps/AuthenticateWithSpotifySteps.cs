using TechTalk.SpecFlow;

namespace Spotkick.Specs.Steps
{
    [Binding]
    public class AuthenticateWithSpotifySteps
    {
        [Given(@"I am on the '(.*)' page")]
        public void GivenIAmOnThePage(string page)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"I am redirected to the '(.*)' page")]
        public void ThenIAmRedirectedToThePage(string page)
        {
            ScenarioContext.Current.Pending();
        }
    }
}