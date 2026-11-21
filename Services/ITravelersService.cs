using Models.DTO;

namespace Services;

public interface ITravelersService
{
    public Task<ResponsePageDto<TravelerDto>> ReadTravelersAsync(bool seeded, string filter, int pageNumber, int pageSize);
}
