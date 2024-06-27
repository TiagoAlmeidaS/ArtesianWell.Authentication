using Authentication.Application.UseCases.Authentication.Command.RegisterUser;
using Authentication.Application.UseCases.Authentication.Query.SignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Authentication.Presentation.Controllers;

public class AuthController(ISender mediator): BaseController
{
    
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Authentication", Description = "Register client")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        return await HandleRequestAsync(command, async _ => await mediator.Send(command, CancellationToken.None));
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Authentication", Description = "Login client")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] SignInQuery query)
    {
        return await HandleRequestAsync(query, async _ => await mediator.Send(query, CancellationToken.None));
    }
}