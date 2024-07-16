﻿using System.Text.Json;
using Authentication.Application.Services.Authentication;
using Authentication.Infra.Service.Clients.Keycloak;
using Authentication.Infra.Service.Mapper;
using Authentication.Infra.Service.Services;
using Authentication.Shared.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Authentication.Infra.Service;

public static class InfraServiceDependencyInjection
{
    public static IServiceCollection InfraServiceExtension(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddServices()
            .AddAutomapperProfiles()
            // .AddMiddlewares()
            .AddConfiguration(configuration)
            .AddExternalServices()
            .AddGlobalConfiguration();
            // .AddCacheService(configuration);

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddTransient<IAuthenticationService, AuthenticationService>();

    // public static IServiceCollection AddMiddlewares(this IServiceCollection services) =>
    //     services.AddTransient<RequestBodyLoggingMiddleware>()
    //         .AddTransient<ResponseBodyLoggingMiddleware>()
    //         .AddSingleton<ITelemetryInitializer, TelemetryMiddleware>();

    // public static IApplicationBuilder UseRequestBodyLogging(this IApplicationBuilder builder)
    // {
    //     return builder.UseMiddleware<RequestBodyLoggingMiddleware>();
    // }

    // public static IApplicationBuilder UseResponseBodyLogging(this IApplicationBuilder builder)
    // {
    //     return builder.UseMiddleware<ResponseBodyLoggingMiddleware>();
    // }

    public static IServiceCollection AddAutomapperProfiles(this IServiceCollection services) =>
        services
            .AddAutoMapper(typeof(AuthenticationProfile));

    private static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var apiKeycloak = scopedProvider.GetRequiredService<IOptionsSnapshot<KeycloakConfig>>();

        services.AddHttpClient(KeycloakConsts.GetNameApi, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(apiKeycloak.Value.Timeout);
            client.BaseAddress = new Uri(apiKeycloak.Value.BaseUrl);
        });
        
        services.AddHttpClient(KeycloakConsts.GetNameApiToken, client =>
        {
            client.DefaultRequestHeaders.Add(CommonConsts.Headers.Accept,
                CommonConsts.Headers.AcceptJsonContentValue);
            client.Timeout = TimeSpan.FromSeconds(apiKeycloak.Value.Timeout);
            client.BaseAddress = new Uri(apiKeycloak.Value.BaseUrlAuthorization);
        });

        return services;
    }

    // public static void AddCacheService(this IServiceCollection services, IConfiguration configuration)
    // {
    //     using var scope = services.BuildServiceProvider().CreateScope();
    //     var scopedProvider = scope.ServiceProvider;
    //     var cacheConfig = scopedProvider.GetRequiredService<IOptionsSnapshot<CacheConfig>>();
    //
    //     services.AddTransient<ICache, Cache>();
    //     services.AddStackExchangeRedisCache(options => { options.Configuration = cacheConfig.Value.ConnectionString;});
    // }

    private static IServiceCollection
        AddConfiguration(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<KeycloakConfig>(config =>
            {
                config.ClientId = Environment.GetEnvironmentVariable("KEYCLOAK_ClientId") ?? configuration[$"{nameof(KeycloakConfig)}:ClientId"];
                config.AdminId = Environment.GetEnvironmentVariable("KEYCLOAK_AdminId") ?? configuration[$"{nameof(KeycloakConfig)}:AdminId"];
                config.BaseUrl = Environment.GetEnvironmentVariable("KEYCLOAK_BaseUrl") ?? configuration[$"{nameof(KeycloakConfig)}:BaseUrl"];
                config.BaseUrlAuthorization = Environment.GetEnvironmentVariable("KEYCLOAK_BaseUrlAuthorization") ?? configuration[$"{nameof(KeycloakConfig)}:BaseUrlAuthorization"];
                config.ClientSecret = Environment.GetEnvironmentVariable("KEYCLOAK_ClientSecret") ?? configuration[$"{nameof(KeycloakConfig)}:ClientSecret"];
                config.Timeout = int.TryParse(Environment.GetEnvironmentVariable("KEYCLOAK_Timeout"), out var timeout) ? timeout : configuration.GetValue<int>($"{nameof(KeycloakConfig)}:Timeout", 15);
                config.GrantType = Environment.GetEnvironmentVariable("KEYCLOAK_GrantType") ?? configuration[$"{nameof(KeycloakConfig)}:GrantType"];
                config.Username = Environment.GetEnvironmentVariable("KEYCLOAK_Username") ?? configuration[$"{nameof(KeycloakConfig)}:Username"];
                config.Password = Environment.GetEnvironmentVariable("KEYCLOAK_Password") ?? configuration[$"{nameof(KeycloakConfig)}:Password"];
            });

    private static IServiceCollection AddGlobalConfiguration(this IServiceCollection services)
        => services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });
}