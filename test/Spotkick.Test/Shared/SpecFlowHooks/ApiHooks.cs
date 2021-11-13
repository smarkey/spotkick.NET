using System;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Spotkick.Models;
using Spotkick.Test.UI.TestContext;
using TechTalk.SpecFlow;

namespace Spotkick.Test.Shared.SpecFlowHooks
{
    [Binding]
    public class ApiHooks
    {
        private readonly string connectionString = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build()
            .GetSection("ConnectionStrings")
            .GetSection("Default")
            .Get<string>();

        private readonly User user = new()
        {
            Id = Guid.Empty.ToString(),
            Email = "a@b.com",
            UserName = "a@b.com",
            DisplayName = "Tester",
            SpotifyUserId = "123",
        };

        public ApiHooks(Context context)
        {
        }

        [BeforeScenario("api")]
        public void BeforeApiScenario()
        {
            AfterApiScenario();

            using var connection = new SqlConnection(connectionString);

            const string sql =
                "INSERT INTO AspNetUsers (Id, DisplayName, Email, UserName, SpotifyUserId, AccessFailedCount, TwoFactorEnabled, EmailConfirmed, LockoutEnabled, PhoneNumberConfirmed )Values (@Id, @DisplayName, @Email, @UserName, @SpotifyUserId, @AccessFailedCount, @TwoFactorEnabled, @EmailConfirmed, @LockoutEnabled, @PhoneNumberConfirmed );";

            var affectedRows = connection.Execute(sql, user);

            affectedRows.ShouldBe(1);
        }

        [AfterScenario("api")]
        public void AfterApiScenario()
        {
            using var connection = new SqlConnection(connectionString);

            const string sql = "DELETE From AspNetUsers WHERE Id =  @Id";

            connection.Execute(sql, user);
        }
    }
}