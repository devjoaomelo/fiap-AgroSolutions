using AgroSolutions.Identity.Application.Validators;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Interfaces;

namespace AgroSolutions.Identity.Application.UseCases.Register;

public record RegisterRequest(string Name, string Email, string Password);

public record RegisterResponse(Guid Id, string Name, string Email, DateTime CreatedAt);

public class RegisterHandler
{
    private readonly IUserRepository _userRepository;

    public RegisterHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request)
    {
        RegisterValidator.Validate(request.Name, request.Email, request.Password);

        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new InvalidOperationException("Email em uso");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(request.Email, passwordHash, request.Name);

        await _userRepository.AddAsync(user);

        return new RegisterResponse(user.Id, user.Name, user.Email, user.CreatedAt);
    }
}