using Microsoft.Extensions.Logging;

using Models.DTO;
using DbRepos;

namespace Services;

public class AttractionsServiceDb : IAttractionsService
{
    private readonly AttractionsDbRepos _repo = null;
    private readonly ILogger<AttractionsServiceDb> _logger = null;

    public AttractionsServiceDb(AttractionsDbRepos repo, ILogger<AttractionsServiceDb> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    //Simple 1:1 calls in this case, but as Services expands, this will no longer need to be the case
    public Task<ResponsePageDto<AttractionDto>> ReadAttractionsAsync(bool seeded, string filter, int pageNumber, int pageSize)
        => _repo.ReadAttractionsAsync(seeded, filter, pageNumber, pageSize);
    public Task<ResponsePageDto<AttractionDto>> ReadAttractionsWithoutCommentsAsync(bool seeded, string filter, int pageNumber, int pageSize)
        => _repo.ReadAttractionsWithoutCommentsAsync(seeded, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<AttractionDetailsDto>> ReadAttractionAsync(Guid id, int pageNumber, int pageSize)
        => _repo.ReadAttractionAsync(id, pageNumber, pageSize);
}
