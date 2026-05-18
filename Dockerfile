 # Use the official .NET 9 runtime as a base image
 FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
 WORKDIR /app
 EXPOSE 8080

 # Use the official .NET 9 SDK image to build the application
 FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
 WORKDIR /src

 # Copy the solution and project files
 COPY ["ASPWebApi.slnx", "./"]
 COPY ["ASPWebApi/ASPWebApi.csproj", "ASPWebApi/"]
 COPY ["Domain/Domain.csproj", "Domain/"]
 COPY ["WebApiTests/WebApiTests.csproj", "WebApiTests/"]
 COPY ["Persistence/Persistence.csproj", "Persistence/"]
 COPY ["Services/Services.csproj", "Services/"]
 COPY ["Services.Abstract/Services.Abstract.csproj", "Services.Abstract/"]

 # Restore dependencies
 RUN dotnet restore

 # Copy the entire source code and build the application
 COPY . .
 WORKDIR /src/ASPWebApi
 RUN dotnet publish "ASPWebApi.csproj" -c Release -o /app/publish

 # Build the runtime image
 FROM base AS final
 WORKDIR /app
 COPY --from=build /app/publish .
 ENTRYPOINT ["dotnet", "ASPWebApi.dll"]