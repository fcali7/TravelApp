using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

[Table("Comments", Schema = "supusr")]
sealed public class CommentDbM : Comment, ISeed<CommentDbM>
{
    [Key]
    public override Guid CommentId { get; set; }

    [Required]
    public override string Text { get; set; }

    [JsonIgnore]
    public Guid AttractionId { get; set; }  // not nullable => Cascade Delete when the Attraction is removed

    [JsonIgnore]
    public Guid TravelerId { get; set; }    // not nullable => Cascade Delete when the Traveler is removed

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override IAttraction Attraction { get => AttractionDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    [ForeignKey("AttractionId")]
    public AttractionDbM AttractionDbM { get; set; } = null;

    [NotMapped]
    public override ITraveler Traveler { get => TravelerDbM; set => new NotImplementedException(); }
    [JsonIgnore]
    [ForeignKey("TravelerId")]
    public TravelerDbM TravelerDbM { get; set; } = null;
    #endregion

    #region randomly seed this instance
    public override CommentDbM Seed(SeedGenerator sgen)
    {
        base.Seed(sgen);
        return this;
    }
    #endregion

    #region constructors
    public CommentDbM() { }
    #endregion
}
