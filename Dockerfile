FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Authentication.Presentation/Authentication.Presentation.csproj", "Authentication.Presentation/"]
COPY ["Authentication.Application/Authentication.Application.csproj", "Authentication.Application/"]
COPY ["Authentication.Infra.Service/Authentication.Infra.Service.csproj", "Authentication.Infra.Service/"]
COPY ["Authentication.Shared/Authentication.Shared.csproj", "Authentication.Shared/"]
RUN dotnet restore "Authentication.Presentation/Authentication.Presentation.csproj"
COPY . .
WORKDIR "/src/Authentication.Presentation"
RUN dotnet build "Authentication.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Authentication.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.Presentation.dll"]
