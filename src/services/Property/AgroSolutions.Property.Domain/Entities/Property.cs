namespace AgroSolutions.Property.Domain.Entities;

public class RuralProperty
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string Location { get; private set; }
    public double TotalArea { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<Field> Fields { get; private set; }

    private RuralProperty()
    {
        Name = string.Empty;
        Location = string.Empty;
        Fields = new List<Field>();
    }

    public RuralProperty(Guid userId, string name, string location, double totalArea)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Location = location;
        TotalArea = totalArea;
        CreatedAt = DateTime.UtcNow;
        Fields = new List<Field>();
    }

    public void Update(string name, string location, double totalArea)
    {
        Name = name;
        Location = location;
        TotalArea = totalArea;
    }
}