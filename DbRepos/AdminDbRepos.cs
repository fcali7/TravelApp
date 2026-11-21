using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;

using Seido.Utilities.SeedGenerator;
using Models.DTO;
using DbModels;
using DbContext;
using Configuration;

namespace DbRepos;

public class AdminDbRepos
{
    private const string _seedSource = "./app-seeds.json";

    //Minimum amount of seeded data according to the project requirements
    private const int _nrOfTravelers = 50;
    private const int _nrOfCities = 100;
    private const int _maxNrOfCommentsPerAttraction = 20;

    private readonly ILogger<AdminDbRepos> _logger;
    private Encryptions _encryptions;
    private readonly MainDbContext _dbContext;

    public AdminDbRepos(ILogger<AdminDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> InfoAsync() => await DbInfo();


    private async Task<ResponseItemDto<GstUsrInfoAllDto>> DbInfo()
    {
        var info = new GstUsrInfoAllDto();
        info.Db = await _dbContext.InfoDbView.FirstAsync();
        info.Countries = await _dbContext.InfoCountriesView.ToListAsync();

        return new ResponseItemDto<GstUsrInfoAllDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif

            Item = info
        };
    }

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> SeedAsync(int nrOfAttractions)
    {
        //Robust seeding: first of all make sure the database is cleared from all seeded data,
        //so the seed can be run any number of times without creating duplicates
        await RemoveSeedAsync(true);

        //Create a seeder
        var fn = Path.GetFullPath(_seedSource);
        var seeder = new SeedGenerator(fn);

        //Travelers (the users writing comments)
        var travelers = seeder.ItemsToList<TravelerDbM>(_nrOfTravelers);

        //Cities, randomly distributed over the countries in the seed file.
        //Unique on (Name, Country) to match the unique index in the database
        var cities = new List<CityDbM>();
        for (int tries = 0; cities.Count < _nrOfCities && tries < 10_000; tries++)
        {
            var city = new CityDbM().Seed(seeder);
            if (!cities.Any(c => c.Name == city.Name && c.Country == city.Country))
                cities.Add(city);
        }

        //Attractions, randomly placed on addresses in the cities
        var attractions = seeder.ItemsToList<AttractionDbM>(nrOfAttractions);
        foreach (var attraction in attractions)
        {
            attraction.CityDbM = seeder.FromList(cities);
            attraction.StreetAddress = seeder.StreetAddress(attraction.CityDbM.Country);

            //Each attraction gets 0 - 20 comments, written by random travelers
            attraction.CommentsDbM = seeder.ItemsToList<CommentDbM>(seeder.Next(0, _maxNrOfCommentsPerAttraction + 1));
            foreach (var comment in attraction.CommentsDbM)
            {
                comment.TravelerDbM = seeder.FromList(travelers);
            }
        }

        //Note that Cities, Comments and Travelers are added through the Attraction navigation properties.
        //Travelers and Cities are also added explicitly, so those without attractions/comments are saved too
        _dbContext.Travelers.AddRange(travelers);
        _dbContext.Cities.AddRange(cities);
        _dbContext.Attractions.AddRange(attractions);

        await _dbContext.SaveChangesAsync();
        return await DbInfo();
    }

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded)
    {
        // Create parameters based on database provider
        var connection = _dbContext.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;

        List<DbParameter> parameters;
        if (connection is MySqlConnection)
        {
            command.CommandText = "supusr_spDeleteAll";
            parameters = new List<DbParameter>
            {
                new MySqlParameter("seededParam", seeded),
                new MySqlParameter("nrTravelersAffected", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
                new MySqlParameter("nrCitiesAffected", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
                new MySqlParameter("nrAttractionsAffected", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
                new MySqlParameter("nrCommentsAffected", MySqlDbType.Int32) { Direction = ParameterDirection.Output }
            };
        }
        else if (connection is NpgsqlConnection)
        {
            // PostgreSQL parameters - call as function returning table
            command.CommandText = "SELECT nrTravelersAffected, nrCitiesAffected, nrAttractionsAffected, nrCommentsAffected FROM supusr.\"spDeleteAll\"(@seededParam)";
            command.CommandType = CommandType.Text;
            parameters =
            [
                new NpgsqlParameter("seededParam", seeded),
                new NpgsqlParameter("nrTravelersAffected", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output },
                new NpgsqlParameter("nrCitiesAffected", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output },
                new NpgsqlParameter("nrAttractionsAffected", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output },
                new NpgsqlParameter("nrCommentsAffected", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output }
            ];
        }
        else
        {
            // SQL Server parameters (default)
            command.CommandText = "supusr.spDeleteAll";
            parameters = new List<DbParameter>
            {
                new SqlParameter("seededParam", seeded),
                new SqlParameter("nrTravelersAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("nrCitiesAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("nrAttractionsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("nrCommentsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
        }

        command.Parameters.AddRange(parameters.ToArray());

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        if (connection is NpgsqlConnection)
        {
            // in postgresql, execute a procedure (a function) cannot return a dataset and have output parameters
            // therefore I execute the command without expecting a result set
            await command.ExecuteScalarAsync();
        }
        else
        {
            // Execute the stored procedure, the result set is not used in this example
            using var reader = await command.ExecuteReaderAsync();
            await reader.CloseAsync();
        }

        _logger.LogInformation($"{nameof(RemoveSeedAsync)}: stored procedure spDeleteAll executed");
        return await DbInfo();
    }

    public async Task<ResponseItemDto<UsrInfoDto>> SeedUsersAsync(int nrOfUsers, int nrOfSuperUsers, int nrOfDbOwners)
    {
        _logger.LogInformation($"Seeding {nrOfUsers} users and {nrOfSuperUsers} superusers");
        
        //First delete all existing users
        foreach (var u in _dbContext.Users)
            _dbContext.Users.Remove(u);

        //add users
        for (int i = 1; i <= nrOfUsers; i++)
        {
            _dbContext.Users.Add(new UserDbM
            {
                UserId = Guid.NewGuid(),
                UserName = $"user{i}",
                Email = $"user{i}@gmail.com",
                Password = _encryptions.EncryptPasswordToBase64($"user{i}"),
                UserRole = "usr"
            });
        }

        //add super user
        for (int i = 1; i <= nrOfSuperUsers; i++)
        {
            _dbContext.Users.Add(new UserDbM
            {
                UserId = Guid.NewGuid(),
                UserName = $"superuser{i}",
                Email = $"superuser{i}@gmail.com",
                Password = _encryptions.EncryptPasswordToBase64($"superuser{i}"),
                UserRole = "supusr"
            });
        }

        //add system adminitrators
        for (int i = 1; i <= nrOfDbOwners; i++)
        {
            _dbContext.Users.Add(new UserDbM
            {
                UserId = Guid.NewGuid(),
                UserName = $"dbo{i}",
                Email = $"dbo{i}@gmail.com",
                Password = _encryptions.EncryptPasswordToBase64($"dbo{i}"),
                UserRole = "dbo"
            });
        }
        await _dbContext.SaveChangesAsync();

        var _info = new UsrInfoDto
        {
            NrUsers = await _dbContext.Users.CountAsync(i => i.UserRole == "usr"),
            NrSuperUsers = await _dbContext.Users.CountAsync(i => i.UserRole == "supusr"),
            NrDbOwners = await _dbContext.Users.CountAsync(i => i.UserRole == "dbo")
        };

        return new ResponseItemDto<UsrInfoDto>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = _info
        };
    }
}
