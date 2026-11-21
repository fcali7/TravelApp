using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class AttractionsDbRepos
{
    private readonly ILogger<AttractionsDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AttractionsDbRepos(ILogger<AttractionsDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    //All attractions, filtered on category, title, description, country and city
    public async Task<ResponsePageDto<AttractionDto>> ReadAttractionsAsync(bool seeded, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";

        //remove tracking for all read operations for performance
        var query = _dbContext.Attractions.AsNoTracking()

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.strCategory.ToLower().Contains(filter) ||
                         i.Title.ToLower().Contains(filter) ||
                         i.Description.ToLower().Contains(filter) ||
                         i.CityDbM.Country.ToLower().Contains(filter) ||
                         i.CityDbM.Name.ToLower().Contains(filter)));

        return await ToPageAsync(query, pageNumber, pageSize);
    }

    //All attractions that do not have any comment
    public async Task<ResponsePageDto<AttractionDto>> ReadAttractionsWithoutCommentsAsync(bool seeded, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";

        var query = _dbContext.Attractions.AsNoTracking()
            .Where(i => (i.Seeded == seeded) && !i.CommentsDbM.Any() &&
                        (i.strCategory.ToLower().Contains(filter) ||
                         i.Title.ToLower().Contains(filter) ||
                         i.Description.ToLower().Contains(filter) ||
                         i.CityDbM.Country.ToLower().Contains(filter) ||
                         i.CityDbM.Name.ToLower().Contains(filter)));

        return await ToPageAsync(query, pageNumber, pageSize);
    }

    //One attraction with category, title, description and all its comments (paged)
    public async Task<ResponseItemDto<AttractionDetailsDto>> ReadAttractionAsync(Guid id, int pageNumber, int pageSize)
    {
        var item = await _dbContext.Attractions.AsNoTracking()
            .Where(i => i.AttractionId == id)
            .Select(i => new AttractionDetailsDto
            {
                AttractionId = i.AttractionId,
                Category = i.strCategory,
                Title = i.Title,
                Description = i.Description,
                StreetAddress = i.StreetAddress,
                City = i.CityDbM.Name,
                Country = i.CityDbM.Country
            })
            .FirstOrDefaultAsync();

        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        var comments = _dbContext.Comments.AsNoTracking()
            .Where(c => c.AttractionId == id);

        item.Comments = new ResponsePageDto<AttractionCommentDto>
        {
            DbItemsCount = await comments.CountAsync(),

            PageItems = await comments
                .OrderByDescending(c => c.Created)

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .Select(c => new AttractionCommentDto
                {
                    CommentId = c.CommentId,
                    Text = c.Text,
                    Created = c.Created,
                    TravelerId = c.TravelerId,
                    TravelerName = c.TravelerDbM.FirstName + " " + c.TravelerDbM.LastName
                })
                .ToListAsync(),

            PageNr = pageNumber,
            PageSize = pageSize
        };

        return new ResponseItemDto<AttractionDetailsDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item
        };
    }

    //Counts all items matching the query, then returns one page of them
    private async Task<ResponsePageDto<AttractionDto>> ToPageAsync(IQueryable<AttractionDbM> query, int pageNumber, int pageSize)
    {
        return new ResponsePageDto<AttractionDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),

            PageItems = await query
                .OrderBy(i => i.Title)
                .ThenBy(i => i.AttractionId)

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .Select(i => new AttractionDto
                {
                    AttractionId = i.AttractionId,
                    Category = i.strCategory,
                    Title = i.Title,
                    Description = i.Description,
                    StreetAddress = i.StreetAddress,
                    City = i.CityDbM.Name,
                    Country = i.CityDbM.Country,
                    NrOfComments = i.CommentsDbM.Count()
                })
                .ToListAsync(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
    }
}
