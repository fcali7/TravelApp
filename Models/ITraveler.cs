namespace Models;

public interface ITraveler
{
    public Guid TravelerId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    // One Traveler may write many Comments
    public List<IComment> Comments { get; set; }
}
