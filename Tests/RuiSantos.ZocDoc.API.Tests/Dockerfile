FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /src
COPY . .
RUN dotnet publish "Server/RuiSantos.ZocDoc.Api/RuiSantos.ZocDoc.Api.csproj" -c Release -o /app /p:UseAppHost=false

WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "RuiSantos.ZocDoc.Api.dll"]