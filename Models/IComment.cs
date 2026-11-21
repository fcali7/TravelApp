namespace Models;

public interface IComment
{
    public Guid CommentId { get; set; }

    public string Text { get; set; }
    public DateTime Created { get; set; }

    // One Comment belongs to exactly one Attraction
    public IAttraction Attraction { get; set; }

    // One Comment is written by exactly one Traveler
    public ITraveler Traveler { get; set; }
}
