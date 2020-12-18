#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Rhisis.WorldServer/Rhisis.WorldServer.csproj", "src/Rhisis.WorldServer/"]
COPY ["src/Rhisis.Scripting/Rhisis.Scripting.csproj", "src/Rhisis.Scripting/"]
COPY ["src/Rhisis.Game.Abstractions/Rhisis.Game.Abstractions.csproj", "src/Rhisis.Game.Abstractions/"]
COPY ["src/Rhisis.Core/Rhisis.Core.csproj", "src/Rhisis.Core/"]
COPY ["src/Rhisis.Game.Common/Rhisis.Game.Common.csproj", "src/Rhisis.Game.Common/"]
COPY ["src/Rhisis.Game/Rhisis.Game.csproj", "src/Rhisis.Game/"]
COPY ["src/Rhisis.Network/Rhisis.Network.csproj", "src/Rhisis.Network/"]
COPY ["src/Rhisis.Database/Rhisis.Database.csproj", "src/Rhisis.Database/"]
COPY ["src/Rhisis.Redis/Rhisis.Redis.csproj", "src/Rhisis.Redis/"]
RUN dotnet restore "src/Rhisis.WorldServer/Rhisis.WorldServer.csproj"
COPY . .
WORKDIR "/src/src/Rhisis.WorldServer"
RUN dotnet build "Rhisis.WorldServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rhisis.WorldServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rhisis.WorldServer.dll"]