namespace Models.DTO;

//Overview of the database content, read from the view gstusr.vwInfoDb
public class GstUsrInfoDbDto
{
    public int NrSeededTravelers { get; set; } = 0;
    public int NrUnseededTravelers { get; set; } = 0;

    public int NrSeededCities { get; set; } = 0;
    public int NrUnseededCities { get; set; } = 0;

    public int NrSeededAttractions { get; set; } = 0;
    public int NrUnseededAttractions { get; set; } = 0;
    public int NrAttractionsWithoutComments { get; set; } = 0;

    public int NrSeededComments { get; set; } = 0;
    public int NrUnseededComments { get; set; } = 0;
}

//Number of cities and attractions per country, read from the view gstusr.vwInfoCountries
public class GstUsrInfoCountriesDto
{
    public string Country { get; set; } = null;
    public int NrCities { get; set; } = 0;
    public int NrAttractions { get; set; } = 0;
}

public class GstUsrInfoAllDto
{
    public GstUsrInfoDbDto Db { get; set; } = null;
    public List<GstUsrInfoCountriesDto> Countries { get; set; } = null;
}
