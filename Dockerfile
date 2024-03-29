# Use the ASP.NET base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project file to the working directory
COPY ["MicroServicePayment/MicroServicePayment.csproj", "MicroServicePayment/"]

# Restore NuGet packages
# Add additional diagnostic output for troubleshooting
RUN dotnet restore "MicroServicePayment/MicroServicePayment.csproj" --source https://api.nuget.org/v3/index.json \
    && echo "NuGet configuration:" \
    && cat /root/.nuget/NuGet/NuGet.Config

# Copy the rest of the source code
COPY . .

# Change working directory to the project folder
WORKDIR "/src/MicroServicePayment"

# Build the application
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
