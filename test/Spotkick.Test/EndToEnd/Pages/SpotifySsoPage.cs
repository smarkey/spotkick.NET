using OpenQA.Selenium;
using Spotkick.Test.EndToEnd.TestContext;

namespace Spotkick.Test.EndToEnd.Pages
{
    public class SpotifySsoPage
    {
        private readonly IWebDriver _driver;

        public SpotifySsoPage(Context context)
        {
            _driver = context.Driver;
        }
        
        public string Title() => _driver.Title;
    }
}