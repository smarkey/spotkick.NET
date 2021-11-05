using Spotkick.Test.UI.TestContext;
using TechTalk.SpecFlow;
using static Spotkick.Test.UI.Infrastructure.WebDriverFactory;

namespace Spotkick.Test.UI.Hooks
{
    [Binding]
    public class CommonHooks
    {
        private readonly Context _context;

        public CommonHooks(Context context)
        {
            _context = context;
        }

        [BeforeScenario("ui")]
        public void BeforeScenario()
        {
            _context.Driver = Chrome();
        }

        [AfterScenario("ui")]
        public void AfterScenario()
        {
            _context.Driver.Close();
            _context.Driver.Quit();
            _context.Driver = null;
        }
    }
}