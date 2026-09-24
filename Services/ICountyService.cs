using MST_4G.Dtos;

namespace MST_4G.Services;

public interface ICountyService
{
    Task<CountyReadDto?> GetByCountyNoAsync(int CountyNo);
    Task<CountyReadDto> CreateCountyAsync(CountyCreateDto createDto);
}