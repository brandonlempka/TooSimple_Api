#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TooSimple_Api/TooSimple_Api.csproj", "TooSimple_Api/"]
COPY ["TooSimple_Database/TooSimple_Database.csproj", "TooSimple_Database/"]
RUN dotnet restore "TooSimple_Api/TooSimple_Api.csproj"
COPY . .
WORKDIR "/src/TooSimple_Api"
RUN dotnet build "TooSimple_Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TooSimple_Api.csproj" -c Release -o /app/publish -r linux-arm64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TooSimple_Api.dll"]