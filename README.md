# What is Spotkick?
Spotkick creates a Spotify **playlist** featuring your **favourite musicians** who are **playing in your area**.</p>

# How does it achieve this goal?
- **Songkick**: Used for Artist/Area performance data
- **Spotify**: Used to establish the user's Followed Artists & to generate the Playlist

# Pre-requisites

## API Dependencies
To run the application you'll need the following config in either `appsettings.json` or set as environment variables:
- [**API key** for **Songkick**](https://www.songkick.com/api_key_requests/new)
- [**Client Secrets** and **Redirect URIs** for **Spotify**](https://developer.spotify.com/dashboard/applications)

If running with Docker you can add the following to the `spotkick` environment instructions in `docker-compose.yml`:
```
environment:
  SPOTKICK_Spotify.ClientId: <REPLACE_ME>
  SPOTKICK_Spotify.ClientSecret: <REPLACE_ME>
  SPOTKICK_Spotify.CallbackUrl: <REPLACE_ME>
  SPOTKICK_Songkick.Key: <REPLACE_ME>
```

## Database

### Setup
Only required if running locally rather than in Docker.

1. Launch the SQL Server Configuration Manager.
2. Select the SQL Server 2005 Network Configuration tab. There should be a Protocols for SQLExpress option, and one of the protocols should be TCP IP.
3. Enable the TCP IP protocol if it is not enabled.
4. The default port for SQL Server Express may not be 1433**. To find the port it is listening on, right-click on the TCP IP protocol and scroll all the way down to the IP All heading.
Make sure to restart SQL Express before trying to connect.

** You'll need the port number (dynamically assigned) in order to view the contents in Rider

### Migrations
Spotkick utilises Entity Framework.

You can generate a migration (diffs the current model compared with the DB schema):
```
cd src\Spotkick
dotnet ef migrations add InitialCreate
```
If you generate a migration script, the application runs your migrations on startup, but you can manually run them using the CLI:
```
dotnet ef database update <date>_InitialCreate
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

The application is available at `http://localhost:5000` and the API is available using swagger at `http://localhost:5000/swagger`

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