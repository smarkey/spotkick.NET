using Spotkick.Test.EndToEnd.TestContext;
using TechTalk.SpecFlow;
using static Spotkick.Test.EndToEnd.Infrastructure.WebDriverFactory;

namespace Spotkick.Test.EndToEnd.Hooks
{
    [Binding]
    public class CommonHooks
    {
        private readonly Context _context;

        public CommonHooks(Context context)
        {
            _context = context;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _context.Driver = Chrome();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _context.Driver.Close();
            _context.Driver.Quit();
            _context.Driver = null;
        }
    }
}