using System.Linq.Expressions;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MST_4G.Data;
using MST_4G.Dtos;
using MST_4G.Extensions;
using MST_4G.Models;

namespace MST_4G.Services;

public class ZoService(AppDbContext context) : IZoService
{

    private static void ValidateZoBusinessRules(params (string? value, int maxLength)[] fields)
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

        public async Task<ZoReadDto?> GetByZoNoAsync(string zoNo)
    {
        return await context.Zo
            .AsNoTracking()
            .Where(c => c.ZoNo == zoNo.Trim())
            .Select(c => new ZoReadDto
            {
                ZoNo = c.ZoNo,
                ZoName = c.ZoName
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ZoReadDto> CreateZoAsync(ZoCreateDto zoCreateDto)
    {

        ValidateZoBusinessRules(
        (zoCreateDto.ZoNo, 8),
        (zoCreateDto.ZoName, 50)
        );

        string zoNo = zoCreateDto.ZoNo.Trim();
        string zoName = zoCreateDto.ZoName.Trim();

        var existingZo = await context.Zo
            .FirstOrDefaultAsync(c => c.ZoNo == zoNo);

        if (existingZo != null)
        {
            throw new ArgumentException($"Zone with number '{zoNo}' already exists.");
        }

        var newZo = new Zo
        {
            ZoNo = zoNo,
            ZoName = zoName
        };

        context.Zo.Add(newZo);
        await context.SaveChangesAsync();

        return new ZoReadDto
        {
            ZoNo = newZo.ZoNo,
            ZoName = newZo.ZoName,
            ZoId = newZo.ZoId
        };
    }
}