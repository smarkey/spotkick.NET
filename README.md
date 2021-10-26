# spotkick.NET

## Database Migrations
`dotnet ef migrations add InitialCreate`

`dotnet ef database update <date>_InitialCreate`

## SQLExpress Setup
You'll need the port number (dynamically assigned) in order to view the contents in Rider
1. Launch the SQL Server Configuration Manager.
2. Select the SQL Server 2005 Network Configuration tab. There should be a Protocols for SQLExpress option, and one of the protocols should be TCP IP.
3. Enable the TCP IP protocol if it is not enabled.
4. The default port for SQL Express may not be 1433. To find the port it is listening on, right-click on the TCP IP protocol and scroll all the way down to the IP All heading.
Make sure to restart SQL Express before trying to connect.
   
## SonarQube Analysis
1. Pull the SonarQube image and spin up a container
    ```
    docker pull sonarqube
    docker run -d --name sonarqube -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true -p 9000:9000 sonarqube:latest
    ```
1. Install SonarScanner
    ```
    dotnet tool install --global dotnet-sonarscanner --version 5.2.1
    ```
1. Set up a project 
   1. Navigate to `http://localhost:9000`
   1. Sign in with `admin:admin` and reset password
   1. Create a new project and generate an Auth key
1. Run SonarScanner to publish analysis to SonarQube
   ```
   dotnet sonarscanner begin /k:"<projectKey>" /d:sonar.login="<token>"
   dotnet build
   dotnet sonarscanner end /d:sonar.login="<token>"
   ```