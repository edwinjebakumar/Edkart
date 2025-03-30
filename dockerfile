# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the solution file and all project files
COPY *.sln ./
COPY Core/Core.csproj Core/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY API/API.csproj API/

# Restore dependencies for the entire solution
RUN dotnet restore Edkart.sln

# Copy all other files
COPY . .

# Publish the app
WORKDIR /src/API
RUN dotnet publish API.csproj -c Release -o /app/out

# Final stage - copy the published files and set the entry point
FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "API.dll"]
