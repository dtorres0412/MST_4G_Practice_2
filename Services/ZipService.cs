using System.Linq.Expressions;
using ClosedXML.Excel;
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
        return await ApplyZipFilters(searchDto)
            .OrderBy(z => z.ZipNo)
            .Select(ToDto)
            .ToPagedResultAsync(searchDto.PageIndex, searchDto.PageSize);
    }

  public async Task<ZipReadDto?> GetByZipNoAsync(string zipNo)
{
    return await context.Zip
        .AsNoTracking()
        .Where(z => z.ZipNo == zipNo.Trim())
        .Select(ToDto)
        .FirstOrDefaultAsync();
}

    public async Task<byte[]> ExportZipsToExcelAsync(ZipSearchDto searchDto)
    {
        var zips = await ApplyZipFilters(searchDto)
            .OrderBy(z => z.ZipNo)
            .Select(ToDto)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Zip_Maintenance_Table");

        worksheet.Cell(1, 1).Value = "ZipId";
        worksheet.Cell(1, 2).Value = "ZipNo";
        worksheet.Cell(1, 3).Value = "ZipName";
        worksheet.Cell(1, 4).Value = "EffDateFrom";
        worksheet.Cell(1, 5).Value = "EffDateTo";
        worksheet.Cell(1, 6).Value = "CountyNo";
        worksheet.Cell(1, 7).Value = "CountyName";
        worksheet.Cell(1, 8).Value = "ZoNo";
        worksheet.Cell(1, 9).Value = "ZoName";
        worksheet.Cell(1, 10).Value = "DoNo";
        worksheet.Cell(1, 11).Value = "DoName";

        for (int i = 0; i < zips.Count; i++)
        {
            var zip = zips[i];
            worksheet.Cell(i + 2, 1).Value = zip.ZipId;
            worksheet.Cell(i + 2, 2).Value = zip.ZipNo;
            worksheet.Cell(i + 2, 3).Value = zip.ZipName;
            worksheet.Cell(i + 2, 4).Value = zip.EffDateFrom;
            worksheet.Cell(i + 2, 5).Value = zip.EffDateTo;
            worksheet.Cell(i + 2, 6).Value = zip.CountyNo;
            worksheet.Cell(i + 2, 7).Value = zip.CountyName;
            worksheet.Cell(i + 2, 8).Value = zip.ZoNo;
            worksheet.Cell(i + 2, 9).Value = zip.ZoName;
            worksheet.Cell(i + 2, 10).Value = zip.DoNo;
            worksheet.Cell(i + 2, 11).Value = zip.DoName;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private IQueryable<Zip> ApplyZipFilters(ZipSearchDto searchDto)
    {
        var query = context.Zip.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchDto.ZipNo))
            query = query.Where(z => z.ZipNo.Contains(searchDto.ZipNo.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.ZipName))
            query = query.Where(z => z.ZipName.Contains(searchDto.ZipName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.CountyNo))
            query = query.Where(z => z.County != null && z.County.CountyNo == searchDto.CountyNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.CountyName))
            query = query.Where(z => z.County != null && z.County.CountyName.Contains(searchDto.CountyName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.ZoNo))
            query = query.Where(z => z.Zo != null && z.Zo.ZoNo == searchDto.ZoNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.ZoName))
            query = query.Where(z => z.Zo != null && z.Zo.ZoName.Contains(searchDto.ZoName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.DoNo))
            query = query.Where(z => z.DistrictOffice != null && z.DistrictOffice.DoNo == searchDto.DoNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.DoName))
            query = query.Where(z => z.DistrictOffice != null && z.DistrictOffice.DoName.Contains(searchDto.DoName.Trim()));

        return query;
    }

    private static readonly Expression<Func<Zip, ZipReadDto>> ToDto = z => new ZipReadDto
    {
        ZipId = z.ZipId,
        ZipNo = z.ZipNo,
        ZipName = z.ZipName ?? string.Empty,
        EffDateFrom = z.EffDateFrom,
        EffDateTo = z.EffDateTo,
        CountyNo = z.County.CountyNo ?? string.Empty,
        CountyName = z.County.CountyName ?? string.Empty,
        ZoNo = z.Zo.ZoNo ?? string.Empty,
        ZoName = z.Zo.ZoName ?? string.Empty,
        DoNo = z.DistrictOffice.DoNo ?? string.Empty,
        DoName = z.DistrictOffice.DoName ?? string.Empty
    };

    public async Task<ZipReadDto>CreateZipAsync(ZipCreateDto createDto)
    {
        var zip = new Zip
        {
            ZipNo = createDto.ZipNo.Trim(),
            ZipName = createDto.ZipName.Trim(),
            EffDateFrom = createDto.EffDateFrom,
            EffDateTo = createDto.EffDateTo,
            CountyId = createDto.CountyId,
            ZoId = createDto.ZoId,
            DoId = createDto.DoId
        };

        context.Zip.Add(zip);
        await context.SaveChangesAsync();

        var result = await GetByZipNoAsync(zip.ZipNo);
        return result!;
    }
}