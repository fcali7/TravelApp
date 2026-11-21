namespace Models.DTO;

//Response DTOs used by the read services. They contain exactly the data each service should show,
//which avoids circular references (Attraction -> Comment -> Attraction) and very large json responses.

public class AttractionDto
{
    public Guid AttractionId { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public int NrOfComments { get; set; }
}

//One attraction with its comments, the comments are paged
public class AttractionDetailsDto
{
    public Guid AttractionId { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public ResponsePageDto<AttractionCommentDto> Comments { get; set; }
}

//A comment as shown on an attraction: who wrote it
public class AttractionCommentDto
{
    public Guid CommentId { get; set; }
    public string Text { get; set; }
    public DateTime Created { get; set; }
    public Guid TravelerId { get; set; }
    public string TravelerName { get; set; }
}

//A traveler (user) with all comments the traveler has written
public class TravelerDto
{
    public Guid TravelerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int NrOfComments { get; set; }
    public List<TravelerCommentDto> Comments { get; set; }
}

//A comment as shown on a traveler: which attraction it is about
public class TravelerCommentDto
{
    public Guid CommentId { get; set; }
    public string Text { get; set; }
    public DateTime Created { get; set; }
    public Guid AttractionId { get; set; }
    public string AttractionTitle { get; set; }
}
