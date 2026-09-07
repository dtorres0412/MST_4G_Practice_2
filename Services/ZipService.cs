using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MST_4G.Data;
using MST_4G.Dtos;
using MST_4G.Models;

namespace MST_4G.Services;

public class ZipService(AppDbContext context) : IZipService
{
    public async Task<IEnumerable<ZipReadDto>> SearchZipsAsync(ZipSearchDto searchDto)
    {
        var query = context.Zip.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchDto.ZipNo))
            query = query.Where(z => z.ZipNo.Contains(searchDto.ZipNo));

        if (!string.IsNullOrWhiteSpace(searchDto.ZipName))
            query = query.Where(z => z.ZipName.Contains(searchDto.ZipName));

        if (!string.IsNullOrWhiteSpace(searchDto.CountyNo))
            query = query.Where(z => z.CountyNo == searchDto.CountyNo);

        if (!string.IsNullOrWhiteSpace(searchDto.CountyName))
            query = query.Where(z => z.County != null && z.County.CountyName.Contains(searchDto.CountyName));

        if (!string.IsNullOrWhiteSpace(searchDto.ZoNo))
            query = query.Where(z => z.ZoNo == searchDto.ZoNo);

        if (!string.IsNullOrWhiteSpace(searchDto.ZoName))
            query = query.Where(z => z.Zo != null && z.Zo.ZoName.Contains(searchDto.ZoName));

        if (!string.IsNullOrWhiteSpace(searchDto.DoNo))
            query = query.Where(z => z.DoNo == searchDto.DoNo);

        if (!string.IsNullOrWhiteSpace(searchDto.DoName))
            query = query.Where(z => z.DistrictOffice != null && z.DistrictOffice.DoName.Contains(searchDto.DoName));

        return await query.Select(ToDto).ToListAsync();
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
        ZipNo = z.ZipNo,
        ZipName = z.ZipName,
        EffDateFrom = z.EffDateFrom,
        EffDateTo = z.EffDateTo,
        CountyNo = z.CountyNo,
        CountyName = z.County != null ? z.County.CountyName : string.Empty,
        ZoNo = z.ZoNo,
        ZoName = z.Zo != null ? z.Zo.ZoName : string.Empty,
        DoNo = z.DoNo,
        DoName = z.DistrictOffice != null ? z.DistrictOffice.DoName : string.Empty
    };
}