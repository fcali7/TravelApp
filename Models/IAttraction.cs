namespace Models;

public enum AttractionCategory { Restaurant, Cafe, Architecture, Museum, Park, Viewpoint };

public interface IAttraction
{
    public Guid AttractionId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public AttractionCategory Category { get; set; }
    public string StreetAddress { get; set; }

    // One Attraction is located in exactly one City
    public ICity City { get; set; }

    // One Attraction may have many Comments
    public List<IComment> Comments { get; set; }
}
