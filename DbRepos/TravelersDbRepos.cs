using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Models.DTO;
using DbContext;

namespace DbRepos;

public class TravelersDbRepos
{
    private readonly ILogger<TravelersDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public TravelersDbRepos(ILogger<TravelersDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    //All travelers (users) and the comments each traveler has written
    public async Task<ResponsePageDto<TravelerDto>> ReadTravelersAsync(bool seeded, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";

        //remove tracking for all read operations for performance
        var query = _dbContext.Travelers.AsNoTracking()

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.FirstName.ToLower().Contains(filter) ||
                         i.LastName.ToLower().Contains(filter) ||
                         i.Email.ToLower().Contains(filter)));

        return new ResponsePageDto<TravelerDto>
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),

            PageItems = await query
                .OrderBy(i => i.LastName)
                .ThenBy(i => i.FirstName)
                .ThenBy(i => i.TravelerId)

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .Select(i => new TravelerDto
                {
                    TravelerId = i.TravelerId,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    Email = i.Email,
                    NrOfComments = i.CommentsDbM.Count(),
                    Comments = i.CommentsDbM
                        .OrderByDescending(c => c.Created)
                        .Select(c => new TravelerCommentDto
                        {
                            CommentId = c.CommentId,
                            Text = c.Text,
                            Created = c.Created,
                            AttractionId = c.AttractionId,
                            AttractionTitle = c.AttractionDbM.Title
                        }).ToList()
                })
                .ToListAsync(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
    }
}
