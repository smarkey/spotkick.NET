using System;
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
            switch (page)
            {
                case "Home":
                    _homePage.Go();
                    _homePage.Title().ShouldBe("Welcome - Spotkick");
                    break;
                default:
                    throw new ArgumentException($"{page} is not supported by this BDD Step");
            }
        }

        [When(@"I click on the '(.*)' button")]
        public void WhenIClickOnTheButton(string button)
        {
            switch (button)
            {
                case "Login to Spotify":
                    _homePage.ClickLoginToSpotifyButton();
                    break;
                default:
                    throw new ArgumentException($"{button} is not supported by this BDD Step");
            }
        }

        [Then(@"I am redirected to the '(.*)' page")]
        public void ThenIAmRedirectedToThePage(string page)
        {
            switch (page)
            {
                case "Spotify SSO":
                    _spotifySsoPage.Title().ShouldBe("Login - Spotify");
                    break;
                default:
                    throw new ArgumentException($"{page} is not supported by this BDD Step");
            }
        }
    }
}