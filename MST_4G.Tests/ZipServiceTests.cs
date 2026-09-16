using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MST_4G.Data;
using MST_4G.Dtos;
using MST_4G.Models;
using MST_4G.Services;
using Xunit;

namespace MST_4G.Tests;

public class ZipServiceTests
{
    private async Task<AppDbContext> GetInMemoryDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    // ==========================================
    // 1. GET BY ZIP NO TESTS
    // ==========================================

    [Fact]
    public async Task GetByZipNoAsync_WhenZipExists_ReturnsCorrectDto()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();

        var zipMaster = new Zip { ZipId = 1, ZipNo = "10001", ZipName = "Manila", EffDateFrom = DateTime.UtcNow, EffDateTo = DateTime.UtcNow };
        var county = new County { CountyNo = "C001", CountyName = "Metro Manila" };
        var zo = new Zo { ZoNo = "Z101", ZoName = "Zone 1" };
        var districtOffice = new DistrictOffice { DoNo = "D001", DoName = "District 1" };

        context.ZipJunction.Add(new ZipJunction
        {
            ZipNo = "10001",
            CountyNo = "C001",
            ZoNo = "Z101",
            DoNo = "D001",
            Zip = zipMaster,
            County = county,
            Zo = zo,
            DistrictOffice = districtOffice
        });
        await context.SaveChangesAsync();

        var service = new ZipService(context);

        // Act
        var result = await service.GetByZipNoAsync(" 10001 "); // Pinapasa nang may whitespace para ma-test ang .Trim()

        // Assert
        Assert.NotNull(result);
        Assert.Equal("10001", result.ZipNo);
        Assert.Equal("Manila", result.ZipName);
        Assert.Equal("Metro Manila", result.CountyName);
    }

    [Fact]
    public async Task GetByZipNoAsync_WhenZipDoesNotExist_ReturnsNull()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);

        // Act
        var result = await service.GetByZipNoAsync("99999");

        // Assert
        Assert.Null(result);
    }

    // ==========================================
    // 2. CREATE ZIP TESTS
    // ==========================================

    [Fact]
    public async Task CreateZipAsync_WhenMasterZipDoesNotExist_CreatesNewMasterAndJunction()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();

        context.County.Add(new County { CountyNo = "C001", CountyName = "County A" });
        context.Zo.Add(new Zo { ZoNo = "Z101", ZoName = "Zone A" });
        context.DistrictOffice.Add(new DistrictOffice { DoNo = "D001", DoName = "District A" });
        await context.SaveChangesAsync();

        var service = new ZipService(context);
        var createDto = new ZipCreateDto
        {
            ZipNo = "10002",
            ZipName = "Makati",
            CountyNo = "C001",
            ZoNo = "Z101",
            DoNo = "D001",
            EffDateFrom = DateTime.UtcNow,
            EffDateTo = new DateTime(9999, 12, 31)
        };

        // Act
        var result = await service.CreateZipAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("10002", result.ZipNo);
        Assert.Equal("Makati", result.ZipName);

        var savedZipMaster = await context.Zip.FirstOrDefaultAsync(z => z.ZipNo == "10002");
        Assert.NotNull(savedZipMaster);
    }

    // ==========================================
    // 3. UPDATE ZIP TESTS
    // ==========================================

    [Fact]
    public async Task UpdateZipAsync_WhenZipExists_UpdatesMasterAndReplacesJunction()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();

        var existingZip = new Zip { ZipId = 1, ZipNo = "10001", ZipName = "Old Name", EffDateFrom = DateTime.UtcNow, EffDateTo = DateTime.UtcNow };
        context.Zip.Add(existingZip);

        var county = new County { CountyNo = "C001", CountyName = "County A" };
        var zo = new Zo { ZoNo = "Z101", ZoName = "Zone A" };
        var districtOffice = new DistrictOffice { DoNo = "D001", DoName = "District A" };
        context.County.Add(county);
        context.Zo.Add(zo);
        context.DistrictOffice.Add(districtOffice);

        context.ZipJunction.Add(new ZipJunction { ZipNo = "10001", CountyNo = "C001", ZoNo = "Z101", DoNo = "D001", Zip = existingZip, County = county, Zo = zo, DistrictOffice = districtOffice });
        await context.SaveChangesAsync();

        var service = new ZipService(context);
        var updateDto = new ZipUpdateDto
        {
            ZipNo = "10001",
            ZipName = "Updated Manila Name",
            CountyNo = "C001",
            ZoNo = "Z101",
            DoNo = "D001",
            EffDateFrom = DateTime.UtcNow,
            EffDateTo = new DateTime(9999, 12, 31)
        };

        // Act
        var result = await service.UpdateZipAsync(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Manila Name", result.ZipName);

        var updatedMasterInDb = await context.Zip.FirstAsync(z => z.ZipNo == "10001");
        Assert.Equal("Updated Manila Name", updatedMasterInDb.ZipName);
    }

    [Fact]
    public async Task UpdateZipAsync_WhenZipDoesNotExist_ReturnsNull()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);
        var updateDto = new ZipUpdateDto { ZipNo = "99999", ZipName = "Non Existent", CountyNo = "C001", ZoNo = "Z101", DoNo = "D001" };

        // Act
        var result = await service.UpdateZipAsync(updateDto);

        // Assert
        Assert.Null(result);
    }

    // ==========================================
    // 4. DELETE ZIP TESTS
    // ==========================================

    [Fact]
    public async Task DeleteZipAsync_WhenZipIdExists_ReturnsTrueAndRemovesRecord()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var zipToDelete = new Zip { ZipId = 10, ZipNo = "10003", ZipName = "Pasig", EffDateFrom = DateTime.UtcNow, EffDateTo = DateTime.UtcNow };
        context.Zip.Add(zipToDelete);
        await context.SaveChangesAsync();

        var service = new ZipService(context);

        // Act
        bool result = await service.DeleteZipAsync(10);

        // Assert
        Assert.True(result);
        Assert.Null(await context.Zip.FindAsync(10));
    }

    [Fact]
    public async Task DeleteZipAsync_WhenZipIdDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);

        // Act
        bool result = await service.DeleteZipAsync(9999);

        // Assert
        Assert.False(result);
    }

    // ==========================================
    // 5. EXPORT TO EXCEL TEST
    // ==========================================

    [Fact]
    public async Task ExportZipsToExcelAsync_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);
        var searchDto = new ZipSearchDto();

        // Act
        var excelBytes = await service.ExportZipsToExcelAsync(searchDto);

        // Assert
        Assert.NotNull(excelBytes);
        Assert.True(excelBytes.Length > 0);
    }

    // ==========================================
    // 6. PROCESS RESUME TESTS
    // ==========================================

    [Fact]
    public async Task ProcessResumeAsync_WhenOriItemNotInVitaeList_ReturnsFalse()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);

        var payload = new ZipProcessResumeDto
        {
            NewItem = new ZipCreateDto 
            { 
                ZipNo = "10001", 
                ZipName = "Manila", 
                CountyNo = "C01", 
                ZoNo = "Z01", 
                DoNo = "D01", 
                EffDateFrom = DateTime.UtcNow,
                EffDateTo = new DateTime(9999, 12, 31)
            },
            OriItem = new ZipReadDto { ZipId = 5, ZipNo = "10001" },
            VitaeList = new List<ZipReadDto> { new ZipReadDto { ZipId = 99, ZipNo = "10001" } }
        };

        // Act
        var result = await service.ProcessResumeAsync(payload);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ProcessResumeAsync_WhenValidResume_ReturnsTrueAndUpdatesDates()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();

        DateTime maxDate = DateTime.SpecifyKind(new DateTime(9999, 12, 31), DateTimeKind.Utc);
        var activeZip = new Zip 
        { 
            ZipId = 5, 
            ZipNo = "10001", 
            ZipName = "Old Manila", 
            EffDateFrom = DateTime.UtcNow.AddYears(-1), 
            EffDateTo = maxDate 
        };
        context.Zip.Add(activeZip);
        await context.SaveChangesAsync();

        var service = new ZipService(context);

        var payload = new ZipProcessResumeDto
        {
            NewItem = new ZipCreateDto 
            { 
                ZipNo = "10001", 
                ZipName = "New Manila", 
                CountyNo = "C01", 
                ZoNo = "Z01", 
                DoNo = "D01", 
                EffDateFrom = DateTime.UtcNow 
            },
            OriItem = new ZipReadDto { ZipId = 5, ZipNo = "10001" },
            VitaeList = new List<ZipReadDto> { new ZipReadDto { ZipId = 5, ZipNo = "10001" } }
        };

        // Act
        var result = await service.ProcessResumeAsync(payload);

        // Assert
        Assert.True(result);

        var updatedActiveZip = await context.Zip.FindAsync(5);
        Assert.NotNull(updatedActiveZip);
        Assert.NotEqual(maxDate, updatedActiveZip.EffDateTo);
    }

    // ==========================================
    // 7. BUSINESS RULE VALIDATION TESTS (B231002 & B231003)
    // ==========================================

    [Theory]
    [InlineData("^")]
    [InlineData("<")]
    [InlineData(">")]
    [InlineData("|")]
    [InlineData("&")]
    [InlineData("\"")]
    [InlineData("'")]
    [InlineData(",")]
    public async Task CreateZipAsync_WhenInputHasForbiddenCharacters_ThrowsB231002(string forbiddenChar)
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);
        var invalidDto = new ZipCreateDto
        {
            ZipNo = $"10{forbiddenChar}01",
            ZipName = "Manila",
            CountyNo = "C001",
            ZoNo = "Z101",
            DoNo = "D001"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateZipAsync(invalidDto));
        Assert.Contains("B231002", exception.Message);
    }

    [Fact]
    public async Task CreateZipAsync_WhenZipNoExceedsMaxLength_ThrowsB231003()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);
        var invalidDto = new ZipCreateDto
        {
            ZipNo = "100001", // 6 characters (> max 5)
            ZipName = "Manila",
            CountyNo = "C001",
            ZoNo = "Z101",
            DoNo = "D001"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateZipAsync(invalidDto));
        Assert.Contains("B231003", exception.Message);
    }

    [Fact]
    public async Task SearchZipsAsync_WhenFilterHasForbiddenCharacters_ThrowsB231002()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);
        var searchDto = new ZipSearchDto
        {
            ZipNo = "100^|"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.SearchZipsAsync(searchDto));
        Assert.Contains("B231002", exception.Message);
    }

    [Fact]
    public async Task SearchZipsAsync_WhenCountyNoExceedsMaxLength_ThrowsB231003()
    {
        // Arrange
        var context = await GetInMemoryDbContextAsync();
        var service = new ZipService(context);
        var searchDto = new ZipSearchDto
        {
            CountyNo = "111111111" // 9 characters (> max 8)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.SearchZipsAsync(searchDto));
        Assert.Contains("B231003", exception.Message);
    }
}