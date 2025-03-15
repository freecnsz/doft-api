# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /doft-backend

# Copy solution and restore dependencies (improves caching)
COPY doft-backend.sln .  
COPY src src  

WORKDIR /doft-backend/src/doft.Webapi
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o /app/publish

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "doft.Webapi.dll"]
