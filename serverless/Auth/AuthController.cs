using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Authentication.Application;
using Authentication.Application.UseCases.Authentication.Command.RegisterUser;
using Authentication.Application.UseCases.Authentication.Command.SignIn;
using Authentication.Infra.Service;
using Authentication.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Messages;
using Swashbuckle.AspNetCore.Annotations;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Auth;

public class AuthController: ArtesianWellBaseController
{
    private static IMessageHandlerService _errorWarningHandlingService = new MessageHandlerService();
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;
    
    public AuthController() : base(ServiceProviderInitializer.GetService<IMessageHandlerService>())
    {
        _mediator = ServiceProviderInitializer.GetService<IMediator>();
    }

    [SwaggerOperation(Summary = "Authentication", Description = "Register client")]
    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([Amazon.Lambda.Annotations.APIGateway.FromBody] RegisterUserCommand command, ILambdaContext context)
    {
        // var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var result = await _mediator.Send(command, CancellationToken.None);
        
        context.Logger.LogLine($"Handling the 'Post' Response: ${result.Email}, ${result.Name}");
        return HandleResult(result);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/login")]
    [SwaggerOperation(Summary = "Authentication", Description = "Login client")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([Amazon.Lambda.Annotations.APIGateway.FromBody] SignInQuery query, ILambdaContext context)
    {
        // var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var result = await _mediator.Send(query, CancellationToken.None);
        
        context.Logger.LogLine($"Handling the 'Post' Response: ${result.Data.Email}, ${result.Data.Name}");
        if (result.HasError)
        {
            return BadRequest(result.Errors);
        }
        
        return Ok(result);
    }
}

public static class ServiceProviderInitializer
{
    private static readonly Lazy<IServiceProvider> LazyProvider = new Lazy<IServiceProvider>(InitializeServiceProvider);

    public static IServiceProvider Instance => LazyProvider.Value;

    private static IServiceProvider InitializeServiceProvider()
    {
        var services = new ServiceCollection();

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfiguration configuration = builder.Build();
        services.AddSingleton(configuration);

        services.AddControllers();
        services.AddHttpClient();
        services.InfraServiceExtension(configuration);
        services.ApplicationExtension();
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

        return services.BuildServiceProvider();
    }

    public static T GetService<T>()
    {
        return Instance.GetRequiredService<T>();
    }
}
