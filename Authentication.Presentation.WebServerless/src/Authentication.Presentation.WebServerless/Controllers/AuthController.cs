using Authentication.Application.UseCases.Authentication.Command.RegisterUser;
using Authentication.Application.UseCases.Authentication.Command.SignIn;
using Authentication.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;
using Swashbuckle.AspNetCore.Annotations;

namespace Authentication.Presentation.WebServerless.Controllers;

public class AuthController: ArtesianWellBaseController
{
    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator, IMessageHandlerService messageHandlerService): base(messageHandlerService)
    {
        _mediator = mediator;
    }
    
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Authentication", Description = "Register client")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command, CancellationToken.None);
        return HandleResult(result);
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Authentication", Description = "Login client")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] SignInQuery query)
    {
        var result = await _mediator.Send(query, CancellationToken.None);
        return HandleResult(result);
    }
}