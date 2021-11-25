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

The application is available at `http://localhost:6253` and the API is available using swagger at `http://localhost:6253/swagger`

When using Docker, the port number is `6254`

# Testing & Static Analysis

## Testing
You can use your IDE e.g. click the Play button in VS, VSCode or Rider.

Alternatively you can use the .NET Core CLI:
```
cd test\Spotkick.Test
dotnet test
```
   
## Static Analysis
SonarLint is installed in the project but if you want deeper insights afforded by the SonarQube UI:

1. Pull the SonarQube image and spin up a container
    ```
    docker pull sonarqube
    docker run -d --name sonarqube -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true -p 9000:9000 sonarqube:latest
    ```
2. Install SonarScanner
    ```
    dotnet tool install --global dotnet-sonarscanner
    ```
3. Set up a project 
   1. Navigate to `http://localhost:9000`
   1. Sign in with `admin:admin` and reset password
   1. Create a new project and generate an Auth key
4. Run SonarScanner to publish analysis to SonarQube
   ```
   dotnet sonarscanner begin /k:"<projectKey>" /d:sonar.login="<token>"
   dotnet build
   dotnet sonarscanner end /d:sonar.login="<token>"
   ```
5. Explore the project you created at `http://localhost:9000`