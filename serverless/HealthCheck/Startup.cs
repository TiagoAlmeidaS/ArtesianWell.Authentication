using Amazon.Lambda.Annotations;
using Authentication.Application;
using Authentication.Infra.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Messages;

namespace HealthCheck;

[LambdaStartup]
public class Startup
{
    public static IServiceProvider ServiceProvider { get; private set; }
    /// <summary>
    /// Services for Lambda functions can be registered in the services dependency injection container in this method. 
    ///
    /// The services can be injected into the Lambda function through the containing type's constructor or as a
    /// parameter in the Lambda function using the FromService attribute. Services injected for the constructor have
    /// the lifetime of the Lambda compute container. Services injected as parameters are created within the scope
    /// of the function invocation.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfiguration configuration = builder.Build();
        services.AddSingleton(configuration);

        services.AddControllers();
        services.AddHttpClient();
        services.ApplicationExtension();
        services.InfraServiceExtension(configuration);
        services.AddMessageHandling();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:8080/auth/realms/ArtesianWell";
                options.Audience = "artesianwell-client";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ClientePolicy", policy => policy.RequireRole("cliente"));
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        Console.WriteLine("Estou realizando a inserção de dados no StartUp");
        ServiceProvider = services.BuildServiceProvider();
    }
}