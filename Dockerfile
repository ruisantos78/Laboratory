FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ./Modules ./Modules
COPY ./Server/RuiSantos.Labs.Core ./Server/RuiSantos.Labs.Core
COPY ./Server/RuiSantos.Labs.Api ./Server/RuiSantos.Labs.Api

WORKDIR "/src/Server/RuiSantos.Labs.Api"
RUN dotnet publish . -c Release -o /app/publish /p:UseAppHost=false

# Final image with the published application
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
EXPOSE 80
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RuiSantos.Labs.Api.dll"]
