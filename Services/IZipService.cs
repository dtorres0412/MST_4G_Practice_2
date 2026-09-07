using MST_4G.Dtos;

namespace MST_4G.Services;

public interface IZipService
{
    Task<IEnumerable<ZipReadDto>> SearchZipsAsync(ZipSearchDto searchDto);
    Task<ZipReadDto?>GetByZipNoAsync(string zipNo);
}