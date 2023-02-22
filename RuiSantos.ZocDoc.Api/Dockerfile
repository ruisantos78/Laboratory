#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RuiSantos.ZocDoc.Api/RuiSantos.ZocDoc.Api.csproj", "RuiSantos.ZocDoc.Api/"]
COPY ["RuiSantos.ZocDoc.Core/RuiSantos.ZocDoc.Core.csproj", "RuiSantos.ZocDoc.Core/"]
COPY ["RuiSantos.ZocDoc.Data.Mongodb/RuiSantos.ZocDoc.Data.Mongodb.csproj", "RuiSantos.ZocDoc.Data.Mongodb/"]
RUN dotnet restore "RuiSantos.ZocDoc.Api/RuiSantos.ZocDoc.Api.csproj"
COPY . .
WORKDIR "/src/RuiSantos.ZocDoc.Api"
RUN dotnet build "RuiSantos.ZocDoc.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RuiSantos.ZocDoc.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RuiSantos.ZocDoc.Api.dll"]