FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", ""]
COPY ["Authentication.Presentantion/Authentication.Presentantion.csproj", "Authentication.Presentantion/"]
RUN dotnet restore "Authentication.Presentantion/Authentication.Presentantion.csproj"
COPY . .
WORKDIR "/src/Authentication.Presentantion"
RUN dotnet build "Authentication.Presentantion.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Authentication.Presentantion.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.Presentantion.dll"]
