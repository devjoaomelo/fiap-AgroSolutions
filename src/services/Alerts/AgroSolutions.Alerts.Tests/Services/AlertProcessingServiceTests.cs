using AgroSolutions.Alerts.Application.Services;
using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AgroSolutions.Alerts.Tests.Services;

public class AlertProcessingServiceTests
{
    private readonly Mock<IAlertRepository> _mockRepository;
    private readonly Mock<ILogger<AlertProcessingService>> _mockLogger;
    private readonly AlertProcessingService _service;

    public AlertProcessingServiceTests()
    {
        _mockRepository = new Mock<IAlertRepository>();
        _mockLogger = new Mock<ILogger<AlertProcessingService>>();
        _service = new AlertProcessingService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ProcessSensorDataAsync_ShouldCreateDroughtAlert_WhenSoilMoistureIsLow()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var soilMoisture = 20.0; // Abaixo de 30%
        var temperature = 25.0;
        var precipitation = 0.0;

        _mockRepository
            .Setup(r => r.GetByFieldIdAsync(fieldId, false))
            .ReturnsAsync(new List<Alert>());

        // Act
        await _service.ProcessSensorDataAsync(fieldId, soilMoisture, temperature, precipitation);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(
            It.Is<Alert>(a => a.Type == "Seca" && a.FieldId == fieldId && a.Severity == "Alta")), Times.Once);
    }

    [Fact]
    public async Task ProcessSensorDataAsync_ShouldCreateHighTemperatureAlert_WhenTemperatureIsHigh()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var soilMoisture = 50.0;
        var temperature = 40.0; // Acima de 35°C
        var precipitation = 0.0;

        _mockRepository
            .Setup(r => r.GetByFieldIdAsync(fieldId, false))
            .ReturnsAsync(new List<Alert>());

        // Act
        await _service.ProcessSensorDataAsync(fieldId, soilMoisture, temperature, precipitation);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(
            It.Is<Alert>(a => a.Type == "AltaTemperatura" && a.FieldId == fieldId && a.Severity == "Média")), Times.Once);
    }

    [Fact]
    public async Task ProcessSensorDataAsync_ShouldCreateHeavyRainAlert_WhenPrecipitationIsHigh()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var soilMoisture = 50.0;
        var temperature = 25.0;
        var precipitation = 60.0; // Acima de 50mm

        _mockRepository
            .Setup(r => r.GetByFieldIdAsync(fieldId, false))
            .ReturnsAsync(new List<Alert>());

        // Act
        await _service.ProcessSensorDataAsync(fieldId, soilMoisture, temperature, precipitation);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(
            It.Is<Alert>(a => a.Type == "ChuvaForte" && a.FieldId == fieldId && a.Severity == "Alta")), Times.Once);
    }

    [Fact]
    public async Task ProcessSensorDataAsync_ShouldCreateMultipleAlerts_WhenMultipleConditionsAreMet()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var soilMoisture = 15.0; // Baixa
        var temperature = 38.0;  // Alta
        var precipitation = 55.0; // Alta

        _mockRepository
            .Setup(r => r.GetByFieldIdAsync(fieldId, false))
            .ReturnsAsync(new List<Alert>());

        // Act
        await _service.ProcessSensorDataAsync(fieldId, soilMoisture, temperature, precipitation);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Alert>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ProcessSensorDataAsync_ShouldNotCreateDuplicateAlert_WhenAlertAlreadyExists()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var soilMoisture = 20.0;
        var temperature = 25.0;
        var precipitation = 0.0;

        var existingAlert = new Alert(fieldId, "Seca", "Mensagem", "Alta");

        _mockRepository
            .Setup(r => r.GetByFieldIdAsync(fieldId, false))
            .ReturnsAsync(new List<Alert> { existingAlert });

        // Act
        await _service.ProcessSensorDataAsync(fieldId, soilMoisture, temperature, precipitation);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Alert>()), Times.Never);
    }

    [Fact]
    public async Task ProcessSensorDataAsync_ShouldNotCreateAnyAlert_WhenConditionsAreNormal()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var soilMoisture = 50.0; // Normal
        var temperature = 25.0;  // Normal
        var precipitation = 10.0; // Normal

        _mockRepository
            .Setup(r => r.GetByFieldIdAsync(fieldId, false))
            .ReturnsAsync(new List<Alert>());

        // Act
        await _service.ProcessSensorDataAsync(fieldId, soilMoisture, temperature, precipitation);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Alert>()), Times.Never);
    }
}