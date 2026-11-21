using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

[Table("Cities", Schema = "supusr")]
[Index(nameof(Name), nameof(Country), IsUnique = true)]     // same city name may exist only once per country
[Index(nameof(Country))]                                    // used when filtering attractions on country
sealed public class CityDbM : City, ISeed<CityDbM>
{
    [Key]
    public override Guid CityId { get; set; }

    [Required]
    public override string Name { get; set; }

    [Required]
    public override string Country { get; set; }

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override List<IAttraction> Attractions { get => AttractionsDbM?.ToList<IAttraction>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<AttractionDbM> AttractionsDbM { get; set; } = null;
    #endregion

    #region randomly seed this instance
    public override CityDbM Seed(SeedGenerator sgen)
    {
        base.Seed(sgen);
        return this;
    }
    #endregion

    #region constructors
    public CityDbM() { }
    #endregion
}
