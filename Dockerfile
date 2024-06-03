# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source

# Copy everything and restore as distinct layers
COPY . .

# Restore dependencies for both projects
RUN dotnet restore "./LawSearch_API/LawSearch_API.csproj" --disable-parallel
RUN dotnet restore "./LawSearch_Core/LawSearch_Core.csproj" --disable-parallel

# Publish the API project
RUN dotnet publish "./LawSearch_API/LawSearch_API.csproj" -c Release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app

# Copy the built application
COPY --from=build /app ./

# Expose port 8080
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "LawSearch_API.dll"]
