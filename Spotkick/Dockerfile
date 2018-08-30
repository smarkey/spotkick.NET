FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
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
