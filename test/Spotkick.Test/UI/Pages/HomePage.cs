using OpenQA.Selenium;
using Spotkick.Test.UI.TestContext;

namespace Spotkick.Test.UI.Pages
{
    public class HomePage
    {
        private readonly IWebDriver _driver;
        private static By BtnFetchFollowedArtists => By.CssSelector("[data-test-id='fetch-followed-artists-button']");

        public HomePage(Context context)
        {
            _driver = context.Driver;
        }

        public void Go() => _driver.Navigate().GoToUrl("http://localhost:6254/");
        public string Title() => _driver.Title;

        public void ClickFetchFollowedArtistsButton() => _driver.FindElement(BtnFetchFollowedArtists).Click();
    }
}