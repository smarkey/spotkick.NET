using Spotkick.Test.UI.TestContext;
using TechTalk.SpecFlow;
using static Spotkick.Test.UI.Infrastructure.WebDriverFactory;

namespace Spotkick.Test.Shared.SpecFlowHooks
{
    [Binding]
    public class UiHooks
    {
        private readonly Context _context;

        public UiHooks(Context context)
        {
            _context = context;
        }

        [BeforeScenario("ui")]
        public void BeforeUiScenario()
        {
            _context.Driver = Chrome();
        }

        [AfterScenario("ui")]
        public void AfterUiScenario()
        {
            _context.Driver.Close();
            _context.Driver.Quit();
            _context.Driver = null;
        }
    }
}