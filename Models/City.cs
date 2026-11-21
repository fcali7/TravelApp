using Seido.Utilities.SeedGenerator;

namespace Models;

public class City : ICity, ISeed<City>
{
    public virtual Guid CityId { get; set; }

    public virtual string Name { get; set; }
    public virtual string Country { get; set; }

    // Model relationships
    // One City may have many Attractions
    public virtual List<IAttraction> Attractions { get; set; } = null;

    #region constructors
    public City() { }
    public City(City org)
    {
        this.Seeded = org.Seeded;

        this.CityId = org.CityId;
        this.Name = org.Name;
        this.Country = org.Country;
    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual City Seed(SeedGenerator sgen)
    {
        Seeded = true;
        CityId = Guid.NewGuid();

        Country = sgen.Country;
        Name = sgen.City(Country);

        return this;
    }
    #endregion
}
