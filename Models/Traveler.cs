using Seido.Utilities.SeedGenerator;

namespace Models;

// A Traveler is a user who writes comments about attractions.
// Named Traveler to keep it separate from the login User in the template (dbo.Users).
public class Traveler : ITraveler, ISeed<Traveler>
{
    public virtual Guid TravelerId { get; set; }

    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual string Email { get; set; }

    // Model relationships
    // One Traveler may write many Comments
    public virtual List<IComment> Comments { get; set; } = null;

    #region constructors
    public Traveler() { }
    public Traveler(Traveler org)
    {
        this.Seeded = org.Seeded;

        this.TravelerId = org.TravelerId;
        this.FirstName = org.FirstName;
        this.LastName = org.LastName;
        this.Email = org.Email;
    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual Traveler Seed(SeedGenerator sgen)
    {
        Seeded = true;
        TravelerId = Guid.NewGuid();

        FirstName = sgen.FirstName;
        LastName = sgen.LastName;
        Email = sgen.Email(FirstName, LastName);

        return this;
    }
    #endregion
}
