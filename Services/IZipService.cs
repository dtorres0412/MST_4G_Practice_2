using MST_4G.Dtos;

namespace MST_4G.Services;

public interface IZipService
{
    Task<PagedResult<ZipReadDto>> SearchZipsAsync(ZipSearchDto searchDto);
    Task<ZipReadDto?> GetByZipNoAsync(string zipNo);
    Task<byte[]> ExportZipsToExcelAsync(ZipSearchDto searchDto);
    Task<ZipReadDto> CreateZipAsync(ZipCreateDto createDto);
    Task<ZipReadDto?> UpdateZipAsync(ZipUpdateDto updateDto);
    Task<bool> DeleteZipAsync(int zipId);
    Task<bool> ProcessResumeAsync(ZipProcessResumeDto payload);
}