using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Something went wrong: " + ex.Message });
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

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok (await context.Users.ToListAsync());
    }

    [HttpGet("get-user-by-id/{id:int}")]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(user);    
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "User ID claim not found in token or You didn't authorized!" });
        }
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);


        if (user == null)
            return NotFound(new { message = "User not found" });

        var result = new
        {
            user.Id,
            user.UserName,
            user.PhoneNumber,
            user.Email,
            user.ProfilePhotoUrl,
            user.Bio,
        };

        return Ok(result);
    }
}
