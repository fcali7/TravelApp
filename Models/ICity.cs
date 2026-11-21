namespace Models;

public interface ICity
{
    public Guid CityId { get; set; }

    public string Name { get; set; }
    public string Country { get; set; }

    // One City may have many Attractions
    public List<IAttraction> Attractions { get; set; }
}
