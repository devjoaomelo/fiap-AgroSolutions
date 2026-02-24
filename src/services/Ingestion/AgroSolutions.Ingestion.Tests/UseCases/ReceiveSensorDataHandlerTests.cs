using AgroSolutions.Ingestion.Application.Events;
using AgroSolutions.Ingestion.Application.UseCases.ReceiveSensorData;
using AgroSolutions.Ingestion.Domain.Entities;
using AgroSolutions.Ingestion.Domain.Interfaces;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace AgroSolutions.Ingestion.Tests.UseCases;

public class ReceiveSensorDataHandlerTests
{
    private readonly Mock<ISensorDataRepository> _mockRepository;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly Mock<ILogger<ReceiveSensorDataHandler>> _mockLogger;
    private readonly ReceiveSensorDataHandler _handler;

    public ReceiveSensorDataHandlerTests()
    {
        _mockRepository = new Mock<ISensorDataRepository>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _mockLogger = new Mock<ILogger<ReceiveSensorDataHandler>>();
        _handler = new ReceiveSensorDataHandler(_mockRepository.Object, _mockPublishEndpoint.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldSaveSensorDataAndPublishEvent_WhenDataIsValid()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var request = new ReceiveSensorDataRequest(
            FieldId: fieldId,
            SoilMoisture: 50.0,
            Temperature: 25.0,
            Precipitation: 10.0,
            Timestamp: DateTime.UtcNow
        );

        // Act
        var response = await _handler.Handle(request);

        // Assert
        response.Should().NotBeNull();
        response.FieldId.Should().Be(fieldId);
        response.SoilMoisture.Should().Be(50.0);
        response.Temperature.Should().Be(25.0);
        response.Precipitation.Should().Be(10.0);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<SensorData>()), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(
            It.Is<SensorDataReceivedEvent>(e => e.FieldId == fieldId),
            default), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSoilMoistureIsNegative()
    {
        // Arrange
        var request = new ReceiveSensorDataRequest(
            FieldId: Guid.NewGuid(),
            SoilMoisture: -10.0,
            Temperature: 25.0,
            Precipitation: 10.0
        );

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<SensorData>()), Times.Never);
        _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<SensorDataReceivedEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSoilMoistureIsAbove100()
    {
        // Arrange
        var request = new ReceiveSensorDataRequest(
            FieldId: Guid.NewGuid(),
            SoilMoisture: 150.0,
            Temperature: 25.0,
            Precipitation: 10.0
        );

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<SensorData>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenTemperatureIsTooLow()
    {
        // Arrange
        var request = new ReceiveSensorDataRequest(
            FieldId: Guid.NewGuid(),
            SoilMoisture: 50.0,
            Temperature: -60.0,
            Precipitation: 10.0
        );

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<SensorData>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenTemperatureIsTooHigh()
    {
        // Arrange
        var request = new ReceiveSensorDataRequest(
            FieldId: Guid.NewGuid(),
            SoilMoisture: 50.0,
            Temperature: 70.0,
            Precipitation: 10.0
        );

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<SensorData>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenPrecipitationIsNegative()
    {
        // Arrange
        var request = new ReceiveSensorDataRequest(
            FieldId: Guid.NewGuid(),
            SoilMoisture: 50.0,
            Temperature: 25.0,
            Precipitation: -5.0
        );

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<SensorData>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentTimestamp_WhenTimestampIsNull()
    {
        // Arrange
        var request = new ReceiveSensorDataRequest(
            FieldId: Guid.NewGuid(),
            SoilMoisture: 50.0,
            Temperature: 25.0,
            Precipitation: 10.0,
            Timestamp: null
        );

        // Act
        var response = await _handler.Handle(request);

        // Assert
        response.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task Handle_ShouldPublishEventWithCorrectData()
    {
        // Arrange
        var fieldId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        var request = new ReceiveSensorDataRequest(
            FieldId: fieldId,
            SoilMoisture: 35.0,
            Temperature: 28.0,
            Precipitation: 15.0,
            Timestamp: timestamp
        );

        SensorDataReceivedEvent? publishedEvent = null;
        _mockPublishEndpoint
            .Setup(p => p.Publish(It.IsAny<SensorDataReceivedEvent>(), default))
            .Callback<SensorDataReceivedEvent, CancellationToken>((evt, _) => publishedEvent = evt);

        // Act
        await _handler.Handle(request);

        // Assert
        publishedEvent.Should().NotBeNull();
        publishedEvent!.FieldId.Should().Be(fieldId);
        publishedEvent.SoilMoisture.Should().Be(35.0);
        publishedEvent.Temperature.Should().Be(28.0);
        publishedEvent.Precipitation.Should().Be(15.0);
        publishedEvent.Timestamp.Should().Be(timestamp);
    }
}