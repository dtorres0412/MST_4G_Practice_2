using MST_4G.Dtos;

namespace MST_4G.Services;

public interface IZoService
{
    Task<ZoReadDto?> GetByZoNoAsync(string ZoNo);
    Task<ZoReadDto> CreateZoAsync(ZoCreateDto zoCreateDto);
}