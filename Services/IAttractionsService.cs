using Models.DTO;

namespace Services;

public interface IAttractionsService
{
    public Task<ResponsePageDto<AttractionDto>> ReadAttractionsAsync(bool seeded, string filter, int pageNumber, int pageSize);
    public Task<ResponsePageDto<AttractionDto>> ReadAttractionsWithoutCommentsAsync(bool seeded, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<AttractionDetailsDto>> ReadAttractionAsync(Guid id, int pageNumber, int pageSize);
}
