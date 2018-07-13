FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Spotkick/Spotkick.csproj Spotkick/
RUN dotnet restore Spotkick/Spotkick.csproj
COPY . .
WORKDIR /src/Spotkick
RUN dotnet build Spotkick.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Spotkick.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Spotkick.dll"]
