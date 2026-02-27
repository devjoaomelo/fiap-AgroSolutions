using AgroSolutions.Property.Domain.Interfaces;

namespace AgroSolutions.Property.Application.UseCases.DeleteField;

public sealed record DeleteFieldRequest(Guid FieldId);
public sealed record DeleteFieldResponse(bool Success);

public sealed class DeleteFieldHandler
{
    private readonly IFieldRepository _fieldRepository;

    public DeleteFieldHandler(IFieldRepository fieldRepository)
    {
        _fieldRepository = fieldRepository;
    }

    public async Task<DeleteFieldResponse> Handle(DeleteFieldRequest request)
    {
        // Verificar se field existe
        var field = await _fieldRepository.GetByIdAsync(request.FieldId);
        if (field == null)
            throw new KeyNotFoundException("Talhão não encontrado");

        await _fieldRepository.DeleteAsync(request.FieldId);

        return new DeleteFieldResponse(true);
    }
}