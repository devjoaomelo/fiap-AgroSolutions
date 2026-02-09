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
        // Validar se email já existe
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new InvalidOperationException("Email em uso");
        }

        // Hash da senha
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Criar usuário
        var user = new User(request.Email, passwordHash, request.Name);

        // Salvar user
        await _userRepository.AddAsync(user);

        return new RegisterResponse(user.Id, user.Name, user.Email, user.CreatedAt);
    }
}