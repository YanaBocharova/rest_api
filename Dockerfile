# Use the official .NET 9 runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET 9 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["ASPWebApi/ASPWebApi/ASPWebApi.csproj", "./"]
RUN dotnet restore "./ASPWebApi/ASPWebApi.csproj"

# Copy the rest of the application and build it
COPY . .
RUN dotnet publish "ASPWebApi/ASPWebApi.csproj" -c Release -o /app/publish
# Build the runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ASPWebApi.dll"]