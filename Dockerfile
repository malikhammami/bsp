
# Use the ASP.NET base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project file and restore NuGet packages
COPY ["MicroServicePayment/MicroServicePayment.csproj", "MicroServicePayment/"]
RUN dotnet restore "MicroServicePayment/MicroServicePayment.csproj" --source https://api.nuget.org/v3/index.json

# Copy the rest of the source code and build the application
COPY . .
WORKDIR "/src/MicroServicePayment"
RUN dotnet build "MicroServicePayment.csproj" -c Release -o /app/build

# Create a separate stage for publishing the application
FROM build AS publish
RUN dotnet publish "MicroServicePayment.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the base stage for the final image
FROM base AS final
WORKDIR /app

# Copy the published application from the publish stage
COPY --from=publish /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "MicroServicePayment.dll"]
