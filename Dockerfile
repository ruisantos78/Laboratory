#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Server/RuiSantos.ZocDoc.Api/RuiSantos.ZocDoc.Api.csproj", "Server/RuiSantos.ZocDoc.Api/"]
COPY ["Server/RuiSantos.ZocDoc.Core/RuiSantos.ZocDoc.Core.csproj", "Server/RuiSantos.ZocDoc.Core/"]
COPY ["Modules/RuiSantos.ZocDoc.Data.Mongodb/RuiSantos.ZocDoc.Data.Mongodb.csproj", "Modules/RuiSantos.ZocDoc.Data.Mongodb/"]
COPY ["Modules/RuiSantos.ZocDoc.Data.Dynamodb/RuiSantos.ZocDoc.Data.Dynamodb.csproj", "Modules/RuiSantos.ZocDoc.Data.Dynamodb/"]
RUN dotnet restore "Server/RuiSantos.ZocDoc.Api/RuiSantos.ZocDoc.Api.csproj"
COPY . .
WORKDIR "/src/Server/RuiSantos.ZocDoc.Api"
RUN dotnet build "RuiSantos.ZocDoc.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RuiSantos.ZocDoc.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
EXPOSE 80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RuiSantos.ZocDoc.Api.dll"]