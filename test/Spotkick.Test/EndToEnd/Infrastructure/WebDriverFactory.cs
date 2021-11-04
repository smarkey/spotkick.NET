using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Spotkick.Test.EndToEnd.Infrastructure
{
    public static class WebDriverFactory
    {
        public static IWebDriver Chrome()
        {
            var driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}