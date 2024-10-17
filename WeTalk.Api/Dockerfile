# WeTalk.Api/Dockerfile

# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app

# Copy the project files
COPY WeTalk.Api/WeTalk.Api.csproj ./WeTalk.Api/
COPY WeTalk.Common/WeTalk.Common.csproj ./WeTalk.Common/
COPY WeTalk.Extensions/WeTalk.Extensions.csproj ./WeTalk.Extensions/
COPY WeTalk.Interfaces/WeTalk.Interfaces.csproj ./WeTalk.Interfaces/
COPY WeTalk.Models/WeTalk.Models.csproj ./WeTalk.Models/

RUN dotnet restore WeTalk.Api/WeTalk.Api.csproj

# Copy the rest of the application
COPY . ./

# Copy the appsettings.json and appsettings.Development.json files into the image
# COPY WeTalk.Api/appsettings.json /app/appsettings.json
# COPY WeTalk.Api/appsettings.Development.json /app/appsettings.Development.json

# Build and publish the app
RUN dotnet publish WeTalk.Api/WeTalk.Api.csproj -c Release -o out

# Use the official .NET runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .

# Expose necessary ports
EXPOSE 80

# Set environment variable for Development environment
ENV ASPNETCORE_ENVIRONMENT Development

# Run the app
ENTRYPOINT ["dotnet", "WeTalk.Api.dll"]