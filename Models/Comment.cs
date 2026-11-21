using Seido.Utilities.SeedGenerator;

namespace Models;

public class Comment : IComment, ISeed<Comment>
{
    public virtual Guid CommentId { get; set; }

    public virtual string Text { get; set; }
    public virtual DateTime Created { get; set; }

    // Model relationships
    // One Comment belongs to exactly one Attraction
    public virtual IAttraction Attraction { get; set; } = null;

    // One Comment is written by exactly one Traveler
    public virtual ITraveler Traveler { get; set; } = null;

    #region constructors
    public Comment() { }
    public Comment(Comment org)
    {
        this.Seeded = org.Seeded;

        this.CommentId = org.CommentId;
        this.Text = org.Text;
        this.Created = org.Created;
    }
    #endregion

    #region randomly seed this instance
    public bool Seeded { get; set; } = false;

    public virtual Comment Seed(SeedGenerator sgen)
    {
        Seeded = true;
        CommentId = Guid.NewGuid();

        Text = sgen.Quote.Quote;
        Created = sgen.DateAndTime(2020, 2026);

        return this;
    }
    #endregion
}
