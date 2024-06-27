using Microsoft.AspNetCore.Mvc;

namespace Authentication.Presentation.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class BaseController: ControllerBase
{
    protected async Task<IActionResult> HandleRequestAsync<TInput>(TInput input, Func<TInput, Task<object?>> action)
    {
        try
        {
            return Ok(await action(input));
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}