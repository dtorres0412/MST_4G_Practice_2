using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MST_4G.Data;
using MST_4G.Dtos;
using MST_4G.Extensions;
using MST_4G.Models;

namespace MST_4G.Services;

public class ZipService(AppDbContext context) : IZipService
{
    public async Task<PagedResult<ZipReadDto>> SearchZipsAsync(ZipSearchDto searchDto)
    {
        var query = context.Zip.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchDto.ZipNo))
            query = query.Where(z => z.ZipNo.Contains(searchDto.ZipNo.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.ZipName))
            query = query.Where(z => z.ZipName.Contains(searchDto.ZipName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.CountyNo))
            query = query.Where(z => z.CountyNo == searchDto.CountyNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.CountyName))
            query = query.Where(z => z.County != null && z.County.CountyName.Contains(searchDto.CountyName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.ZoNo))
            query = query.Where(z => z.ZoNo == searchDto.ZoNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.ZoName))
            query = query.Where(z => z.Zo != null && z.Zo.ZoName.Contains(searchDto.ZoName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.DoNo))
            query = query.Where(z => z.DoNo == searchDto.DoNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.DoName))
            query = query.Where(z => z.DistrictOffice != null && z.DistrictOffice.DoName.Contains(searchDto.DoName.Trim()));

        return await query
            .OrderBy(z => z.ZipNo)
            .Select(ToDto)
            .ToPagedResultAsync(searchDto.PageIndex, searchDto.PageSize);
    }

    public async Task<ZipReadDto?> GetByZipNoAsync(string zipNo)
    {
        return await context.Zip
            .AsNoTracking()
            .Where(z => z.ZipNo == zipNo)
            .Select(ToDto)
            .FirstOrDefaultAsync();
    }

    private static readonly Expression<Func<Zip, ZipReadDto>> ToDto = z => new ZipReadDto
{
    ZipNo = z.ZipNo.Trim(),
    ZipName = z.ZipName != null ? z.ZipName.Trim() : string.Empty,
    EffDateFrom = z.EffDateFrom,
    EffDateTo = z.EffDateTo,
    CountyNo = z.CountyNo.Trim(),
    CountyName = z.County != null ? z.County.CountyName.Trim() : string.Empty,
    ZoNo = z.ZoNo.Trim(),
    ZoName = z.Zo != null ? z.Zo.ZoName.Trim() : string.Empty,
    DoNo = z.DoNo.Trim(),
    DoName = z.DistrictOffice != null ? z.DistrictOffice.DoName.Trim() : string.Empty
};
}