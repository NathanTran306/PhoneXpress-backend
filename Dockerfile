# syntax=docker/dockerfile:1

# Build Stage
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source

# Copy project files and restore dependencies first (cached layers)
COPY ./api/*.csproj ./api/
COPY ./application/*.csproj ./application/
COPY ./domain/*.csproj ./domain/
COPY ./infrastructure/*.csproj ./infrastructure/
RUN dotnet restore ./api/*.csproj

# Copy all source files (frequent changes happen here)
COPY . .

# Build the application
WORKDIR /source/api
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -c Release -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy built application from the build stage
COPY --from=build /app .

USER $APP_UID
ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
