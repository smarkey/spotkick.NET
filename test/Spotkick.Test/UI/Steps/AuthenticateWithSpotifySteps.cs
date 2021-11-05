using Shouldly;
using Spotkick.Test.UI.Pages;
using Spotkick.Test.UI.TestContext;
using TechTalk.SpecFlow;

namespace Spotkick.Test.UI.Steps
{
    [Binding]
    public class AuthenticateWithSpotifySteps
    {
        private readonly HomePage _homePage;
        private readonly SpotifySsoPage _spotifySsoPage;

        public AuthenticateWithSpotifySteps(Context context)
        {
            _homePage = new HomePage(context);
            _spotifySsoPage = new SpotifySsoPage(context);
        }

        [Given(@"I am on the '(.*)' page")]
        public void GivenIAmOnThePage(string page)
        {
            _homePage.Go();
            _homePage.Title().ShouldBe("Welcome - Spotkick");
        }

        [When(@"I click on the '(.*)' button")]
        public void WhenIClickOnTheButton(string button)
        {
            _homePage.ClickFetchFollowedArtistsButton();
        }

        [Then(@"I am redirected to the '(.*)' page")]
        public void ThenIAmRedirectedToThePage(string page)
        {
            _spotifySsoPage.Title().ShouldBe("Login - Spotify");
        }
    }
}