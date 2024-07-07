using System.Net;
using Authentication.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Shared.Utils;

public class HandlerResponse: ControllerBase
{
    protected async Task<IActionResult> HandleRequestAsync<TInput>(TInput input, Func<TInput, Task<object>> action)
    {
        try
        {
            return Ok(await action(input));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.CustomErrorObject);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.CustomErrorObject);
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(ex.CustomErrorObject);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}