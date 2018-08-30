﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Spotkick.Specs.Features.Forum
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class ForumRegistrationFeature : Xunit.IUseFixture<ForumRegistrationFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Registration.feature"
#line hidden
        
        public ForumRegistrationFeature()
        {
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Forum Registration", "As an Acorn Answers Visitior\nI want to register\nSo that I can interact with other" +
                    " users", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void SetFixture(ForumRegistrationFeature.FixtureData fixtureData)
        {
        }
        
        void System.IDisposable.Dispose()
        {
            this.ScenarioTearDown();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Forum Registration")]
        [Xunit.TraitAttribute("Description", "An Acorn Answers Visitor can register with an email address and password")]
        public virtual void AnAcornAnswersVisitorCanRegisterWithAnEmailAddressAndPassword()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An Acorn Answers Visitor can register with an email address and password", new string[] {
                        "happy",
                        "e2e"});
#line 7
this.ScenarioSetup(scenarioInfo);
#line 8
 testRunner.Given("I am on the \'Registration\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 9
 testRunner.When("I provide a \'nickname\' that is \'valid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 10
 testRunner.And("I provide an \'email address\' that is \'valid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
 testRunner.And("I provide a \'password\' that is \'valid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
 testRunner.And("I provide a \'repeat password\' that is \'valid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 13
 testRunner.And("I \'accept\' the \'terms and conditions\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 14
 testRunner.When("I click the \'Register\' button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 15
 testRunner.Then("I arrive on the \'Home\' page as a logged in user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 16
 testRunner.And("I receive a \'Welcome\' email", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Forum Registration")]
        [Xunit.TraitAttribute("Description", "An Acorn Answers Visitor can register with SSO")]
        [Xunit.Extensions.InlineDataAttribute("Facebook", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("Twitter", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("LinkedIn", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("Google", new string[0])]
        public virtual void AnAcornAnswersVisitorCanRegisterWithSSO(string socialPlatform, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "happy",
                    "e2e"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An Acorn Answers Visitor can register with SSO", @__tags);
#line 19
this.ScenarioSetup(scenarioInfo);
#line 20
 testRunner.Given("I am on the \'Registration\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 21
 testRunner.When(string.Format("I click the \'{0}\' link", socialPlatform), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 22
 testRunner.And(string.Format("I am diverted to the \'{0}\' service", socialPlatform), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
 testRunner.And(string.Format("I sign in to the \'{0}\' service", socialPlatform), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 24
 testRunner.Then("I arrive on the \'Home\' page as a logged in user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 25
 testRunner.And("I receive a \'Welcome\' email", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Forum Registration")]
        [Xunit.TraitAttribute("Description", "An Acorn Answers Member cannot register using an invalid nickname")]
        [Xunit.Extensions.InlineDataAttribute("special characters", "The nickname needs to consist of A-Z,a-z,0-9 *some* special chars, max 16, no fou" +
            "l language", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("over 16 characters", "The nickname needs to consist of A-Z,a-z,0-9 *some* special chars, max 16, no fou" +
            "l language", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("foul language", "The nickname needs to consist of A-Z,a-z,0-9 *some* special chars, max 16, no fou" +
            "l language", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("blank", "The nickname is a mandatory field", new string[0])]
        public virtual void AnAcornAnswersMemberCannotRegisterUsingAnInvalidNickname(string description, string message, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "component"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An Acorn Answers Member cannot register using an invalid nickname", @__tags);
#line 34
this.ScenarioSetup(scenarioInfo);
#line 35
 testRunner.Given("I am on the \'Registration\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 36
 testRunner.When(string.Format("I provide a nickname that contains \'{0}\'", description), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 37
 testRunner.Then(string.Format("a validation message that reads \'{0}\' is displayed", message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 38
 testRunner.And("the \'Register\' button is \'disabled\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Forum Registration")]
        [Xunit.TraitAttribute("Description", "An Acorn Answers Visitor cannot register using an invalid email address")]
        [Xunit.Extensions.InlineDataAttribute("username missing", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("username too short", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("username greater than 64 characters", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("username contains space", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("missing @", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("mail server missing", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("mail server too short", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("mail server contains space", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("missing .", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("top level domain missing", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("top level domain too short", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("top level domain contains space", "This email address must be a real email address", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("already in use", "This email address is already in use. If you have lost your password, please use " +
            "Forgotton Password link", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("blank", "The email address is a mandatory field", new string[0])]
        public virtual void AnAcornAnswersVisitorCannotRegisterUsingAnInvalidEmailAddress(string description, string message, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "component"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An Acorn Answers Visitor cannot register using an invalid email address", @__tags);
#line 47
this.ScenarioSetup(scenarioInfo);
#line 48
 testRunner.Given("I am on the \'Registration\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 49
 testRunner.When(string.Format("I provide an email address that is \'{0}\'", description), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 50
 testRunner.Then(string.Format("a validation message that reads \'{0}\' is displayed", message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 51
 testRunner.And("the \'Register\' button is \'disabled\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.Extensions.TheoryAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Forum Registration")]
        [Xunit.TraitAttribute("Description", "An Acorn Answers Visitor cannot register using an invalid password")]
        [Xunit.Extensions.InlineDataAttribute("over 10 characters", "The password is invalid. It should be a minimum of 10 characters long", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("does not match", "The password must match", new string[0])]
        [Xunit.Extensions.InlineDataAttribute("blank", "The password is a mandatory field", new string[0])]
        public virtual void AnAcornAnswersVisitorCannotRegisterUsingAnInvalidPassword(string description, string message, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "component"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An Acorn Answers Visitor cannot register using an invalid password", @__tags);
#line 70
this.ScenarioSetup(scenarioInfo);
#line 71
 testRunner.Given("I am on the \'Registration\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 72
 testRunner.When(string.Format("I provide a \'password\' that is \'{0}\'", description), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 73
 testRunner.When(string.Format("I provide a \'repeat password\' that is \'{0}\'", description), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 74
 testRunner.Then(string.Format("a validation message that reads \'{0}\' is displayed", message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 75
 testRunner.And("the \'Register\' button is \'disabled\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Xunit.FactAttribute()]
        [Xunit.TraitAttribute("FeatureTitle", "Forum Registration")]
        [Xunit.TraitAttribute("Description", "An Acorn Answers Visitor cannot register without accepting the terms and conditio" +
            "ns")]
        public virtual void AnAcornAnswersVisitorCannotRegisterWithoutAcceptingTheTermsAndConditions()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An Acorn Answers Visitor cannot register without accepting the terms and conditio" +
                    "ns", new string[] {
                        "component"});
#line 83
this.ScenarioSetup(scenarioInfo);
#line 84
 testRunner.Given("I am on the \'Registration\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 85
 testRunner.And("I provide an email address that is \'valid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 86
 testRunner.And("I provide a password that is \'valid\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 87
 testRunner.When("I \'have not accepted\' the terms and conditions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 88
 testRunner.Then("the \'Register\' button is \'disabled\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                ForumRegistrationFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                ForumRegistrationFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion