using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Application.Services;
using Telegram_V2.Core.Dtos;
using Telegram_V2.Infrastructure.Database;
using Telegram_V2.Application.Exceptions;
using Microsoft.AspNetCore.Identity;
using Telegram_V2.Core.Models;

namespace Telegram_V2.Application.Mediatr.Auth;

public class SignInCommand(SignInRequestDto request) : IRequest<SignInResponseDto>
{
    public SignInRequestDto Request { get; } = request;
}

public class SignInCommandHandler(
    IAuthService authService,
    Context context,
    IPasswordHasher<Users> passwordHasher)
    : IRequestHandler<SignInCommand, SignInResponseDto>
{
    public async Task<SignInResponseDto> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.EmailOrUsername
                                   || u.UserName == request.EmailOrUsername, cancellationToken);
        if (user is null)
        {
            throw new AuthException("Invalid email or password.");
        }

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new AuthException("Invalid email or password.");
        }

        var token = authService.GetToken(user);

        return new SignInResponseDto()
        {
            AccessToken = token,
        };
    }
}