using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

[Table("Attractions", Schema = "supusr")]
[Index(nameof(Title))]          // used when filtering on title
[Index(nameof(strCategory))]    // used when filtering on category
sealed public class AttractionDbM : Attraction, ISeed<AttractionDbM>
{
    [Key]
    public override Guid AttractionId { get; set; }

    [Required]
    public override string Title { get; set; }

    [Required]
    public override string StreetAddress { get; set; }

    [JsonIgnore]
    public Guid CityId { get; set; }    // not nullable => every Attraction must belong to a City

    #region adding more readability to an enum type in the database
    public string strCategory
    {
        get => Category.ToString();
        set { }  //set is needed by EFC to include in the database, so I make it to do nothing
    }
    #endregion

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override ICity City { get => CityDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    [ForeignKey("CityId")]
    public CityDbM CityDbM { get; set; } = null;

    [NotMapped]
    public override List<IComment> Comments { get => CommentsDbM?.ToList<IComment>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<CommentDbM> CommentsDbM { get; set; } = null;
    #endregion

    #region randomly seed this instance
    public override AttractionDbM Seed(SeedGenerator sgen)
    {
        base.Seed(sgen);
        return this;
    }
    #endregion

    #region constructors
    public AttractionDbM() { }
    #endregion
}
