using OpenQA.Selenium;
using Spotkick.Test.UI.TestContext;

namespace Spotkick.Test.UI.Pages
{
    public class HomePage
    {
        private readonly IWebDriver _driver;
        private static By BtnLoginToSpotify => By.CssSelector("[data-test-id='login-to-spotify-button']");

        public HomePage(Context context)
        {
            _driver = context.Driver;
        }

        public void Go() => _driver.Navigate().GoToUrl("http://localhost:6254/");
        public string Title() => _driver.Title;

        public void ClickLoginToSpotifyButton() => _driver.FindElement(BtnLoginToSpotify).Click();
    }
}