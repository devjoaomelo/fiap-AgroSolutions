using AgroSolutions.Identity.Application.UseCases.Register;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AgroSolutions.Identity.Tests.UseCases;

public class RegisterHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly RegisterHandler _handler;

    public RegisterHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new RegisterHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenDataIsValid()
    {
        // Arrange
        var request = new RegisterRequest(
            Name: "João Silva",
            Email: "joao@example.com",
            Password: "Senha@123"
        );

        _mockUserRepository
            .Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        // Act
        var response = await _handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.Name.Should().Be(request.Name);
        response.Email.Should().Be(request.Email.ToLowerInvariant());

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest(
            Name: "João Silva",
            Email: "joao@example.com",
            Password: "Senha@123"
        );

        _mockUserRepository
            .Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act e Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(request));
        exception.Message.Should().Be("Email em uso");

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("", "joao@example.com", "Senha@123")]
    [InlineData(" ", "joao@example.com", "Senha@123")]
    public async Task Handle_ShouldThrowException_WhenNameIsInvalid(string invalidName, string email, string password)
    {
        // Arrange
        var request = new RegisterRequest(
            Name: invalidName,
            Email: email,
            Password: password
        );

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request));

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("João Silva", "invalid-email", "Senha@123")]
    [InlineData("João Silva", "@example.com", "Senha@123")]
    [InlineData("João Silva", "test@", "Senha@123")]
    public async Task Handle_ShouldThrowException_WhenEmailIsInvalid(string name, string invalidEmail, string password)
    {
        // Arrange
        var request = new RegisterRequest(
            Name: name,
            Email: invalidEmail,
            Password: password
        );

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request));

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("João Silva", "joao@example.com", "123")]
    [InlineData("João Silva", "joao@example.com", "senha")]
    [InlineData("João Silva", "joao@example.com", "SENHA")]
    public async Task Handle_ShouldThrowException_WhenPasswordIsWeak(string name, string email, string weakPassword)
    {
        // Arrange
        var request = new RegisterRequest(
            Name: name,
            Email: email,
            Password: weakPassword
        );

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request));

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldHashPassword_BeforeSaving()
    {
        // Arrange
        var request = new RegisterRequest(
            Name: "João Silva",
            Email: "joao@example.com",
            Password: "Senha@123"
        );

        _mockUserRepository
            .Setup(r => r.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        User? capturedUser = null;
        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(request);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser!.PasswordHash.Should().NotBe(request.Password);
        capturedUser.PasswordHash.Should().StartWith("$2");  // BCrypt hash começa com $2
    }
}