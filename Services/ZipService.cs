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
            .OrderBy(zj => zj.ZipNo)
            .Select(ToDto)
            .ToPagedResultAsync(searchDto.PageIndex, searchDto.PageSize);
    }

    public async Task<ZipReadDto?> GetByZipNoAsync(string zipNo)
    {
        return await context.ZipJunction
            .AsNoTracking()
            .Where(zj => zj.ZipNo == zipNo.Trim())
            .Select(ToDto)
            .FirstOrDefaultAsync();
    }

    public async Task<byte[]> ExportZipsToExcelAsync(ZipSearchDto searchDto)
    {
        var zips = await ApplyZipFilters(searchDto)
            .OrderBy(zj => zj.ZipNo)
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

    private IQueryable<ZipJunction> ApplyZipFilters(ZipSearchDto searchDto)
    {
        var query = context.ZipJunction.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchDto.ZipNo))
            query = query.Where(zj => zj.ZipNo.Contains(searchDto.ZipNo.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.ZipName))
            query = query.Where(zj => zj.Zip.ZipName.Contains(searchDto.ZipName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.CountyNo))
            query = query.Where(zj => zj.CountyNo == searchDto.CountyNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.CountyName))
            query = query.Where(zj => zj.County.CountyName.Contains(searchDto.CountyName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.ZoNo))
            query = query.Where(zj => zj.ZoNo == searchDto.ZoNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.ZoName))
            query = query.Where(zj => zj.Zo.ZoName.Contains(searchDto.ZoName.Trim()));

        if (!string.IsNullOrWhiteSpace(searchDto.DoNo))
            query = query.Where(zj => zj.DoNo == searchDto.DoNo.Trim());

        if (!string.IsNullOrWhiteSpace(searchDto.DoName))
            query = query.Where(zj => zj.DistrictOffice.DoName.Contains(searchDto.DoName.Trim()));

        return query;
    }

    private static readonly Expression<Func<ZipJunction, ZipReadDto>> ToDto = zj => new ZipReadDto
    {
        ZipId = zj.Zip.ZipId,
        ZipNo = zj.ZipNo,
        ZipName = zj.Zip.ZipName ?? string.Empty,
        EffDateFrom = zj.Zip.EffDateFrom,
        EffDateTo = zj.Zip.EffDateTo,
        CountyNo = zj.CountyNo,
        CountyName = zj.County.CountyName ?? string.Empty,
        ZoNo = zj.ZoNo,
        ZoName = zj.Zo.ZoName ?? string.Empty,
        DoNo = zj.DoNo,
        DoName = zj.DistrictOffice.DoName ?? string.Empty
    };

    public async Task<ZipReadDto> CreateZipAsync(ZipCreateDto createDto)
    {
        string zipNo = createDto.ZipNo.Trim();

        var zipMaster = await context.Zip.FirstOrDefaultAsync(z => z.ZipNo == zipNo);
        if (zipMaster == null)
        {
            zipMaster = new Zip
            {
                ZipNo = zipNo,
                ZipName = createDto.ZipName.Trim(),
                EffDateFrom = createDto.EffDateFrom,
                EffDateTo = createDto.EffDateTo
            };
            context.Zip.Add(zipMaster);
        }

        var newJunction = new ZipJunction
        {
            ZipNo = zipNo,
            CountyNo = createDto.CountyNo.Trim(),
            ZoNo = createDto.ZoNo.Trim(),
            DoNo = createDto.DoNo.Trim()
        };

        context.ZipJunction.Add(newJunction);
        await context.SaveChangesAsync();

        var result = await context.ZipJunction
            .AsNoTracking()
            .Where(zj => zj.ZipNo == zipNo 
                      && zj.CountyNo == newJunction.CountyNo 
                      && zj.ZoNo == newJunction.ZoNo 
                      && zj.DoNo == newJunction.DoNo)
            .Select(ToDto)
            .FirstOrDefaultAsync();

        return result!;
    }

    public async Task<ZipReadDto?> UpdateZipAsync(ZipUpdateDto updateDto)
    {
        try
        {
            string zipNo = updateDto.ZipNo.Trim();

            var existingZip = await context.Zip.FirstOrDefaultAsync(z => z.ZipNo == zipNo);

            if (existingZip == null)
            {
                return null;
            }

            existingZip.ZipName = updateDto.ZipName.Trim();
            existingZip.EffDateFrom = updateDto.EffDateFrom;
            existingZip.EffDateTo = updateDto.EffDateTo;

            var existingJunctions = await context.ZipJunction
                .Where(zj => zj.ZipNo == zipNo)
                .ToListAsync();

            if (existingJunctions != null && existingJunctions.Count > 0)
            {
                context.ZipJunction.RemoveRange(existingJunctions);
                await context.SaveChangesAsync();
            }

            var newJunction = new ZipJunction();
            newJunction.ZipNo = zipNo;
            newJunction.CountyNo = updateDto.CountyNo.Trim();
            newJunction.ZoNo = updateDto.ZoNo.Trim();
            newJunction.DoNo = updateDto.DoNo.Trim();

            context.ZipJunction.Add(newJunction);
            await context.SaveChangesAsync();

            var updatedData = await context.ZipJunction
                .AsNoTracking()
                .Where(zj => zj.ZipNo == zipNo 
                          && zj.CountyNo == newJunction.CountyNo 
                          && zj.ZoNo == newJunction.ZoNo 
                          && zj.DoNo == newJunction.DoNo)
                .Select(ToDto)
                .FirstOrDefaultAsync();

            return updatedData;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in UpdateZipAsync: " + ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteZipAsync(int zipId)
    {
        var zipRecord = await context.Zip.FindAsync(zipId);

        if (zipRecord == null)
        {
            return false;
        }

        context.Zip.Remove(zipRecord);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ProcessResumeAsync(ZipProcessResumeDto payload)
    {
        try
        {
            var newItem = payload.NewItem;
            var oriItem = payload.OriItem;
            var vitaeList = payload.VitaeList;

            if (oriItem != null)
            {
                bool isFoundInVitaeList = false;
                foreach (var item in vitaeList)
                {
                    if (item.ZipId == oriItem.ZipId)
                    {
                        isFoundInVitaeList = true;
                        break;
                    }
                }

                if (isFoundInVitaeList == false)
                {
                    return false;
                }

                var zipRecordInDatabase = await context.Zip
                    .AsNoTracking()
                    .FirstOrDefaultAsync(z => z.ZipId == oriItem.ZipId);

                if (zipRecordInDatabase == null)
                {
                    return false;
                }
            }

            DateTime dateFromUnspecified = newItem.EffDateFrom.Date;
            DateTime newStartDate = DateTime.SpecifyKind(dateFromUnspecified, DateTimeKind.Utc);

            DateTime maxDateUnspecified = new DateTime(9999, 12, 31);
            DateTime maxDate = DateTime.SpecifyKind(maxDateUnspecified, DateTimeKind.Utc);

            string zipNo = newItem.ZipNo.Trim();

            var activeZip = await context.Zip
                .FirstOrDefaultAsync(z => z.ZipNo == zipNo && z.EffDateTo == maxDate);

            if (activeZip != null)
            {
                activeZip.EffDateTo = newStartDate.AddDays(-1);
                
                await context.SaveChangesAsync();
                context.Entry(activeZip).State = EntityState.Detached;
            }

            var newZip = new Zip();
            newZip.ZipNo = zipNo;
            newZip.ZipName = newItem.ZipName.Trim();
            newZip.EffDateFrom = newStartDate;
            newZip.EffDateTo = maxDate;

            context.Zip.Add(newZip);
            await context.SaveChangesAsync();

            var existingJunctions = await context.ZipJunction
                .Where(zj => zj.ZipNo == zipNo)
                .ToListAsync();

            if (existingJunctions != null && existingJunctions.Count > 0)
            {
                context.ZipJunction.RemoveRange(existingJunctions);
                await context.SaveChangesAsync();
            }

            var newJunction = new ZipJunction();
            newJunction.ZipNo = zipNo;
            newJunction.CountyNo = newItem.CountyNo.Trim();
            newJunction.ZoNo = newItem.ZoNo.Trim();
            newJunction.DoNo = newItem.DoNo.Trim();

            context.ZipJunction.Add(newJunction);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in ProcessResumeAsync: " + ex.Message);
            throw;
        }
    }
}