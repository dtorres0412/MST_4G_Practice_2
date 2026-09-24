using System.Linq.Expressions;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MST_4G.Data;
using MST_4G.Dtos;
using MST_4G.Extensions;
using MST_4G.Models;

namespace MST_4G.Services;

public class CountyService(AppDbContext context) : ICountyService
{

    private static void ValidateCountyBusinessRules(params (string? value, int maxLength)[] fields)
    {
        char[] forbiddenChars = ['^', '<', '>', '|', '&', '"', '\'', ','];

        foreach (var (value, maxLength) in fields)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;

            string trimmedValue = value.Trim();

            if (trimmedValue.IndexOfAny(forbiddenChars) >= 0)
            {
                throw new ArgumentException("B231002:en_此欄位不可使用特殊符號如(^,<,>,|,&,\",')");
            }

            if (trimmedValue.Length > maxLength)
            {
                throw new ArgumentException($"B231003:en_ 超過欄位最大字元長度{maxLength}");
            }
        }
    }

        public async Task<CountyReadDto?> GetByCountyNoAsync(int countyNo)
    {
        return await context.County
            .AsNoTracking()
            .Where(c => c.CountyNo == countyNo)
            .Select(c => new CountyReadDto
            {
                CountyNo = c.CountyNo,
                CountyName = c.CountyName
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CountyReadDto> CreateCountyAsync(CountyCreateDto createCountyDto)
    {

        ValidateCountyBusinessRules(
        (createCountyDto.CountyName, 50)
        );

        int countyNo = createCountyDto.CountyNo;
        string countyName = createCountyDto.CountyName?.Trim() ?? "";

        var existingCounty = await context.County
            .FirstOrDefaultAsync(c => c.CountyNo == countyNo);

        if (existingCounty != null)
        {
            throw new ArgumentException($"County with number '{countyNo}' already exists.");
        }

        var newCounty = new County
        {
            CountyNo = countyNo,
            CountyName = countyName
        };

        context.County.Add(newCounty);
        await context.SaveChangesAsync();

        return new CountyReadDto
        {
            CountyNo = newCounty.CountyNo,
            CountyName = newCounty.CountyName,
            CountyId = newCounty.CountyId
        };
    }
}