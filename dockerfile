# Use the official .NET 6 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project file and restore the dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files and publish the app
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Use the official .NET 6 runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory for the runtime
WORKDIR /app

# Copy the published output from the build image
COPY --from=build /app/out .

# Expose the port that your application will run on
EXPOSE 80

# Set the command to run your app
ENTRYPOINT ["dotnet", "API.dll"]
