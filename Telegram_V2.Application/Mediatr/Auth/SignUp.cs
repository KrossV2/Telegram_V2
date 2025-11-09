using MediatR;
using Microsoft.AspNetCore.Identity;
using Telegram_V2.Core.Dtos;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;

namespace Telegram_V2.Application.Mediatr.Auth;


public class SignUpCommand(UserCreateDto request) : IRequest<UserDto>
{
    public UserCreateDto Request { get; } = request;
}

public class SignUpCommandHandler (Context context, IPasswordHasher<Users> passwordHasher) : IRequestHandler<SignUpCommand, UserDto>
{
    public async Task<UserDto> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var user = new Users()
        {
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            ProfilePhotoUrl = request.ProfilePhotoUrl,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
        };

        user.Password = passwordHasher.HashPassword(user, request.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            ProfilePhotoUrl = user.ProfilePhotoUrl,
            Bio = user.Bio,
            FirstName = user.FirstName,
            LastName = user.LastName,
            LastSeen = DateTime.Now
        };
    }
}
