using Authentication.Application.UseCases.Authentication.Command.RegisterUser;
using Authentication.Application.UseCases.Authentication.Command.SignIn;
using Authentication.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;
using Swashbuckle.AspNetCore.Annotations;

namespace Authentication.Presentation.Controllers;

public class AuthController(
    IMessageHandlerService errorWarningHandlingService,
    IMediator mediator): ArtesianWellBaseController(errorWarningHandlingService)
{
    
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Authentication", Description = "Register client")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await mediator.Send(command, CancellationToken.None);
        return HandleResult(result);
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Authentication", Description = "Login client")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] SignInQuery query)
    {
        var result = await mediator.Send(query, CancellationToken.None);
        return HandleResult(result);
    }
}