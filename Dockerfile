# WeTalk.Api/Dockerfile

# Use the official .NET SDK image for the build environment
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app

# Install ICU (International Components for Unicode) to support culture-specific globalization
RUN apk add --no-cache icu-libs

# Copy the project files for WeTalk.Api and dependencies
COPY WeTalk.Api/WeTalk.Api.csproj ./WeTalk.Api/
COPY WeTalk.Common/WeTalk.Common.csproj ./WeTalk.Common/
COPY WeTalk.Extensions/WeTalk.Extensions.csproj ./WeTalk.Extensions/
COPY WeTalk.Interfaces/WeTalk.Interfaces.csproj ./WeTalk.Interfaces/
COPY WeTalk.Models/WeTalk.Models.csproj ./WeTalk.Models/
COPY WeTalk.Web/WeTalk.Web.csproj ./WeTalk.Web/

# Restore dependencies
RUN dotnet restore WeTalk.Api/WeTalk.Api.csproj
RUN dotnet restore WeTalk.Web/WeTalk.Web.csproj

# Copy the rest of the application source code
# COPY . ./
# Copy all source files from respective directories
COPY WeTalk.Api/ ./WeTalk.Api/
COPY WeTalk.Common/ ./WeTalk.Common/
COPY WeTalk.Extensions/ ./WeTalk.Extensions/
COPY WeTalk.Interfaces/ ./WeTalk.Interfaces/
COPY WeTalk.Models/ ./WeTalk.Models/
COPY WeTalk.Web/ ./WeTalk.Web/

# Build and publish the app
RUN dotnet publish WeTalk.Api/WeTalk.Api.csproj -c Release -o out
RUN dotnet publish WeTalk.Web/WeTalk.Web.csproj -c Release -o web-out

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app

# Install ICU libraries on the runtime container
RUN apk add --no-cache icu-libs

# Disable globalization-invariant mode to allow for culture-specific features
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Copy the published output from the build environment
COPY --from=build-env /app/out .
COPY --from=build-env /app/web-out .

# Expose necessary port
EXPOSE 80
EXPOSE 5001

# Set environment variable for Development environment
ENV ASPNETCORE_ENVIRONMENT Production

# Ensure Swagger and Upfile features are properly configured in appsettings
COPY WeTalk.Web/appsettings.json /app/appsettings.json
COPY WeTalk.Web/appsettings.Development.json /app/appsettings.Development.json

# Run the app
ENTRYPOINT ["dotnet", "WeTalk.Api.dll"]