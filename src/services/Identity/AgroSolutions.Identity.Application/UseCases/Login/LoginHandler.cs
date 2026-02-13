using AgroSolutions.Identity.Application.Services;
using AgroSolutions.Identity.Domain.Interfaces;

namespace AgroSolutions.Identity.Application.UseCases.Login;

public record LoginRequest(string Email, string Password);

public record LoginResponse(Guid UserId, string Name, string Email, string Token);

public class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Credenciais Inválidas");
        }

        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Credenciais Inválidas");
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Name, user.Email);

        return new LoginResponse(user.Id, user.Name, user.Email, token);
    }
}