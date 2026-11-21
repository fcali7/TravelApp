using Microsoft.Extensions.Logging;

using Models.DTO;
using DbRepos;

namespace Services;

public class TravelersServiceDb : ITravelersService
{
    private readonly TravelersDbRepos _repo = null;
    private readonly ILogger<TravelersServiceDb> _logger = null;

    public TravelersServiceDb(TravelersDbRepos repo, ILogger<TravelersServiceDb> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    //Simple 1:1 call in this case, but as Services expands, this will no longer need to be the case
    public Task<ResponsePageDto<TravelerDto>> ReadTravelersAsync(bool seeded, string filter, int pageNumber, int pageSize)
        => _repo.ReadTravelersAsync(seeded, filter, pageNumber, pageSize);
}
