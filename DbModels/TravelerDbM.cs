using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

[Table("Travelers", Schema = "supusr")]
[Index(nameof(LastName), nameof(FirstName))]
sealed public class TravelerDbM : Traveler, ISeed<TravelerDbM>
{
    [Key]
    public override Guid TravelerId { get; set; }

    [Required]
    public override string FirstName { get; set; }

    [Required]
    public override string LastName { get; set; }

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override List<IComment> Comments { get => CommentsDbM?.ToList<IComment>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<CommentDbM> CommentsDbM { get; set; } = null;
    #endregion

    #region randomly seed this instance
    public override TravelerDbM Seed(SeedGenerator sgen)
    {
        base.Seed(sgen);
        return this;
    }
    #endregion

    #region constructors
    public TravelerDbM() { }
    #endregion
}
