using Seido.Utilities.SeedGenerator;

namespace Models;

public class Attraction : IAttraction, ISeed<Attraction>
{
    public virtual Guid AttractionId { get; set; }

    public virtual string Title { get; set; }
    public virtual string Description { get; set; }
    public virtual AttractionCategory Category { get; set; }
    public virtual string StreetAddress { get; set; }

    // Model relationships
    // One Attraction is located in exactly one City
    public virtual ICity City { get; set; } = null;

    // One Attraction may have many Comments
    public virtual List<IComment> Comments { get; set; } = null;

    #region constructors
    public Attraction() { }
    public Attraction(Attraction org)
    {
        this.Seeded = org.Seeded;

        this.AttractionId = org.AttractionId;
        this.Title = org.Title;
        this.Description = org.Description;
        this.Category = org.Category;
        this.StreetAddress = org.StreetAddress;
    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual Attraction Seed(SeedGenerator sgen)
    {
        Seeded = true;
        AttractionId = Guid.NewGuid();

        Category = sgen.FromEnum<AttractionCategory>();
        Title = $"{sgen.LastName}s {Category}";
        Description = sgen.LatinSentence;

        // The street address is set again when the Attraction is placed in a City,
        // so that the street belongs to the same country as the City
        StreetAddress = sgen.StreetAddress();

        return this;
    }
    #endregion
}
