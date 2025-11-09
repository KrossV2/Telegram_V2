using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Application.Exceptions;
using Telegram_V2.Application.Mediatr.Auth;
using Telegram_V2.Core.Dtos;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;

namespace Telegram_V2.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator , Context context) : ControllerBase
{
    [HttpPost("signin")]
    public async Task<ActionResult<SignInResponseDto>> SignIn([FromBody] SignInRequestDto request)
    {
        try
        {
            var result = await mediator.Send(new SignInCommand(request));
            return Ok(result);
        }
        catch (AuthException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("signup")]
    public async Task<ActionResult<UserDto>> SignUp([FromBody] UserCreateDto request)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var result = await mediator.Send(new SignUpCommand(request));
            await transaction.CommitAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(new { message = "Something Went Wrong! : " + ex.Message });
        }
    }
}
