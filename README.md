# What is Spotkick?
Spotkick creates a Spotify **playlist** featuring your **favourite musicians** who are **playing in your area**.</p>

# How does it achieve this goal?
- **Songkick**: For Artist or Area event data.
- **Spotify**: For user's Followed Artists & creation of the Playlist.

# Pre-requisites

## API Dependencies
To run the application you'll need the following config in `appsettings.json` or `appsettings.*.json`:
- [**API key** for **Songkick**](https://www.songkick.com/api_key_requests/new)
- [**Client Id & Secret** and **Callback URLs** for **Spotify**](https://developer.spotify.com/dashboard/applications)

## Database

### Setup
Required for running locally. Not required for running with Docker.

1. Launch the SQL Server Configuration Manager.
2. Select the SQL Server 2005 Network Configuration tab. There should be a Protocols for SQLExpress option, and one of the protocols should be TCP IP.
3. Enable the TCP IP protocol if it is not enabled.
4. The default port for SQL Server Express may not be 1433**. To find the port it is listening on, right-click on the TCP IP protocol and scroll all the way down to the IP All heading.
Make sure to restart SQL Express before trying to connect.

** You'll need the port number (dynamically assigned) in order to view the contents in Rider

### Migrations
Spotkick uses Entity Framework.

You can generate a migration (diffs the current model compared with the DB schema):
```
cd src\Spotkick
dotnet ef migrations add <MigrationScriptName>
```

If you generate a migration script, the application runs them at startup, but you can manually run them using the CLI:
```
dotnet ef database update <GeneratedDate>_<MigrationScriptName>
```

# Running
You can use your IDE e.g. click the Play button in VS, VSCode or Rider.

Alternatively you can use the .NET Core CLI:
```
cd src\Spotkick
dotnet run
```

Or use Docker (Ensure the Docker Daemon is running first on your machine):
```
docker-compose up
```

The application is available at http://localhost:6253 and the API is available using swagger at http://localhost:6253/swagger

When using Docker, the application is available at http://localhost:6254

# Quality & Testing

## Testing
You can use your IDE e.g. click the Play button in VS, VSCode or Rider.

Alternatively you can use the .NET Core CLI:
```
cd test\Spotkick.Test
dotnet test
```

The test framework contains the following scopes of test that are C# & .NET Core:
- Unit tests (in-memory SUT & mocks/stubs of internal services)
- Integration tests (in-memory SUT with mocks of external services)
- UI tests (locally running SUT, driven by Selenium)
- API tests (locally running SUT, driven by HTTP Client)
- Accessibility tests (Roadmap Item)

The test framework also offers the following test scopes via tools than can be run inside docker containers (`docker-compose.test.yml -f docker-compose.test.yml up`):
- Performance tests:
  - Locust (http://localhost:8089) can be used for Load/Stress/Performance testing. A default test script is included.
- Security tests:
  - OWASP Zap (http://localhost:8090) can be used for DAST (Dynamic Application Security Testing) - (Automation as a Roadmap Item)
- Static Analysis:
  - SonarQube (http://localhost:9000) can be used for more in-depth static analysis and security metrics than the SonarLint nuget package in-project
- Continuous Integration:
  - Jenkins (http://localhost:8088) can be configured to provide a pipeline that, among other things, runs tests on-push
   
### SonarQube
SonarLint is installed in the project but if you want deeper insights afforded by the SonarQube UI:

1. Install SonarScanner
    ```
    dotnet tool install --global dotnet-sonarscanner
    ```
2. Set up a project 
   1. Navigate to http://localhost:9000
   1. Sign in with `admin:admin` and reset password
   1. Create a new project and generate an Auth key
3. Run SonarScanner to publish analysis to SonarQube
   ```
   dotnet sonarscanner begin /k:"<projectKey>" /d:sonar.login="<token>"
   dotnet build
   dotnet sonarscanner end /d:sonar.login="<token>"
   ```
4. Review the project you created at `http://localhost:9000/dashboard?id=<projectKey>`

### Locust
1. Review `locust\locustfile.py`
2. Navigate to http://localhost:8089
3. Set the number of Users and the Hatch Rate
4. Click `Start Swarming`