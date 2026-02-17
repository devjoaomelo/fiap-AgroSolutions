using AgroSolutions.Ingestion.Domain.Interfaces;

namespace AgroSolutions.Ingestion.Application.UseCases.GetSensorData;

public record GetSensorDataRequest(Guid FieldId, DateTime? StartDate = null, DateTime? EndDate = null);

public record SensorDataDto(
    Guid SensorDataId,
    double SoilMoisture,
    double Temperature,
    double Precipitation,
    DateTime Timestamp);

public record GetSensorDataResponse(List<SensorDataDto> Data);

public class GetSensorDataHandler
{
    private readonly ISensorDataRepository _sensorDataRepository;

    public GetSensorDataHandler(ISensorDataRepository sensorDataRepository)
    {
        _sensorDataRepository = sensorDataRepository;
    }

    public async Task<GetSensorDataResponse> Handle(GetSensorDataRequest request)
    {
        var sensorData = await _sensorDataRepository.GetByFieldIdAsync(
            request.FieldId,
            request.StartDate,
            request.EndDate);

        var dataDto = sensorData.Select(s => new SensorDataDto(
            s.Id,
            s.SoilMoisture,
            s.Temperature,
            s.Precipitation,
            s.Timestamp
        )).ToList();

        return new GetSensorDataResponse(dataDto);
    }
}