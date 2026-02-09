using AgroSolutions.Identity.Domain.Interfaces;

namespace AgroSolutions.Identity.Application.UseCases.Login;

public record LoginRequest(string Email, string Password);

public record LoginResponse(Guid UserId, string Name, string Email, string Token);

public class LoginHandler
{
    private readonly IUserRepository _userRepository;

    public LoginHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<LoginResponse> Handle(LoginRequest request)
    {
        // Buscar usuário
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Credenciais Inválidas");
        }

        // Verificar senha
        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Credenciais Inválidas");
        }

        // Gerar token JWT (TODO)
        var token = "temporary-token";

        return new LoginResponse(user.Id, user.Name, user.Email, token);
    }
}