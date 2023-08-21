# Use a more recent version of the base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

# Copy the necessary project files and restore dependencies
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Server/RuiSantos.ZocDoc.Api/RuiSantos.ZocDoc.Api.csproj"

# Build and publish the application
WORKDIR "/src/Server/RuiSantos.ZocDoc.Api"
RUN dotnet publish "RuiSantos.ZocDoc.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image with the published application
FROM base AS final
EXPOSE 80
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RuiSantos.ZocDoc.Api.dll"]
