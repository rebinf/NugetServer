#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NugServer/NugServer.csproj", "NugServer/"]
RUN dotnet restore "NugServer/NugServer.csproj"
COPY . .
WORKDIR "/src/NugServer"
RUN dotnet build "NugServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NugServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV NUG_URL="http://localhost:5078"
ENV REQ_KEY="false"
ENV API_KEY="1234567890"
ENV DEL_ALL="false"
ENV DEL_MIN="false"
ENV DEL_PRE="false"

ENTRYPOINT ["dotnet", "NugServer.dll"]