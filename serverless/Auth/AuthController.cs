using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Authentication.Application.UseCases.Authentication.Command.RegisterUser;
using Authentication.Application.UseCases.Authentication.Command.SignIn;
using Authentication.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messages;
using Swashbuckle.AspNetCore.Annotations;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Auth;

public class AuthController: ArtesianWellBaseController
{
    private static IMessageHandlerService _errorWarningHandlingService = new MessageHandlerService();
    private readonly ISender _mediator;
    private readonly IServiceProvider _serviceProvider;
    
    public AuthController() : base(_errorWarningHandlingService)
    {
        _serviceProvider = Startup.ServiceProviderInitializer();
    }

    [SwaggerOperation(Summary = "Authentication", Description = "Register client")]
    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([Amazon.Lambda.Annotations.APIGateway.FromBody] RegisterUserCommand command, ILambdaContext context)
    {
        var mediator = _serviceProvider.GetService<ISender>();
        var result = await mediator.Send(command, CancellationToken.None);
        
        context.Logger.LogLine($"Handling the 'Post' Response: ${result.Email}, ${result.Name}");
        return HandleResult(result);
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/login")]
    [SwaggerOperation(Summary = "Authentication", Description = "Login client")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([Amazon.Lambda.Annotations.APIGateway.FromBody] SignInQuery query, ILambdaContext context)
    {
        var mediator = _serviceProvider.GetService<ISender>();
        var result = await mediator.Send(query, CancellationToken.None);
        
        context.Logger.LogLine($"Handling the 'Post' Response: ${result.Data.Email}, ${result.Data.Name}");
        if (result.HasError)
        {
            return BadRequest(result.Errors);
        }
        
        return Ok(result);
    }
}