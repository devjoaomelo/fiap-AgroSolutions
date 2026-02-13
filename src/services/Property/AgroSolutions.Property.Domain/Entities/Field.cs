namespace AgroSolutions.Property.Domain.Entities;

public class Field
{
    public Guid Id { get; private set; }
    public Guid RuralPropertyId { get; private set; }
    public string Name { get; private set; }
    public string Culture { get; private set; }
    public double Area { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public RuralProperty RuralProperty { get; private set; }

    private Field()
    {
        Name = string.Empty;
        Culture = string.Empty;
        RuralProperty = null!;
    }

    public Field(Guid ruralPropertyId, string name, string culture, double area)
    {
        Id = Guid.NewGuid();
        RuralPropertyId = ruralPropertyId;
        Name = name;
        Culture = culture;
        Area = area;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string culture, double area)
    {
        Name = name;
        Culture = culture;
        Area = area;
    }
}