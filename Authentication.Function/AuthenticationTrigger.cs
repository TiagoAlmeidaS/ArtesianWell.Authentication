using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Authentication.Application.UseCases.Authentication.Command.RegisterUser;
using Authentication.Application.UseCases.Authentication.Command.SignIn;
using Authentication.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Messages;

namespace Authentication.Function;

public class AuthenticationTrigger: HandlerResponse
{
    
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public AuthenticationTrigger(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    [FunctionName("AuthenticationRegisterFunction")]
    public async Task<IActionResult> RegisterAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/register")] HttpRequest req, ILogger log)
    {
        try
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RegisterUserCommand query = JsonConvert.DeserializeObject<RegisterUserCommand>(requestBody);
            
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            return await HandleRequestAsync(query, async i => await mediator.Send(query, CancellationToken.None));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
    
    
    [FunctionName("AuthenticationLoginFunction")]
    public async Task<IActionResult> LoginAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/login")] HttpRequest req, ILogger log)
    {
        try
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SignInQuery query = JsonConvert.DeserializeObject<SignInQuery>(requestBody);
            
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            return await HandleRequestAsync(query, async i => await mediator.Send(query, CancellationToken.None));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}