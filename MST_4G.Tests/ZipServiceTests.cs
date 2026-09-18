using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MST_4G.Data;
using MST_4G.Dtos;
using MST_4G.Models;
using MST_4G.Services;
using Xunit;

namespace MST_4G.Tests;

public class ZipServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ZipService _service;

    public static IEnumerable<object[]> GetNonExistentZipNumbers()
    {
        yield return new object[] { "9999" };
        yield return new object[] { "9889" };
        yield return new object[] { "0000" };
    }

    public static IEnumerable<object[]> GetInvalidZipIds()
    {
        yield return new object[] { 9999 };
        yield return new object[] { 8888 };
        yield return new object[] { 21 };
     }

    public static IEnumerable<object[]> DeleteZipIds()
    {
        yield return new object[] { 1 };
        yield return new object[] { 24 };
        yield return new object[] { 66 };
     }

    public static IEnumerable<object[]> GetForbiddenCharacters()
    {
        yield return new object[] { "^" };
        yield return new object[] { "<" };
        yield return new object[] { ">" };
        yield return new object[] { "|" };
        yield return new object[] { "&" };
        yield return new object[] { "\"" };
        yield return new object[] { "'" };
        yield return new object[] { "," };
    }

    // ==========================================
    // 1. SETUP
    // ==========================================
    public ZipServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        // Seed Baseline Master Data
        SeedTestData();

        _service = new ZipService(_context);
    }

    private void SeedTestData()
    {
        var zips = new List<Zip>
        {
            new Zip { ZipId = 1, ZipNo = "10001", ZipName = "Manila", EffDateFrom = DateTime.UtcNow, EffDateTo = new DateTime(9999, 12, 31) },
            new Zip { ZipId = 24, ZipNo = "10024", ZipName = "Makati", EffDateFrom = DateTime.UtcNow, EffDateTo = new DateTime(9999, 12, 31) },
            new Zip { ZipId = 66, ZipNo = "10066", ZipName = "Pasig", EffDateFrom = DateTime.UtcNow, EffDateTo = new DateTime(9999, 12, 31) }
        };
        var county = new County { CountyNo = "C001", CountyName = "Metro Manila" };
        var zo = new Zo { ZoNo = "Z101", ZoName = "Zone 1" };
        var districtOffice = new DistrictOffice { DoNo = "D001", DoName = "District 1" };

        _context.Zip.AddRange(zips);
        _context.County.Add(county);
        _context.Zo.Add(zo);
        _context.DistrictOffice.Add(districtOffice);

        _context.ZipJunction.Add(new ZipJunction
        {
            ZipNo = "10001",
            CountyNo = "C001",
            ZoNo = "Z101",
            DoNo = "D001",
            Zip = zips[0],
            County = county,
            Zo = zo,
            DistrictOffice = districtOffice
        });

        _context.SaveChanges();
    }

    // ==========================================
    // 2. TEARDOWN
    // ==========================================
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // ==========================================
    // 3. GET BY ZIP NO TESTS
    // ==========================================

    [Fact]
    public async Task GetByZipNoAsync_WhenZipExists_ReturnsCorrectDto()
    {
        // Act
        var result = await _service.GetByZipNoAsync(" 10001 ");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("10001", result.ZipNo);
        Assert.Equal("Manila", result.ZipName);
        Assert.Equal("Metro Manila", result.CountyName);
    }

    [Theory]
    [MemberData(nameof(GetNonExistentZipNumbers))]
    //Value from yield return { "9999" } Pass because it will be Null
    //Value from yield return { "9889" } Pass because it will be Null
    //Value from yield return { "0000" } Pass because it will be Null
    //Value from yield return { "10001" } will fail because it was existing in the predefined data in the RAM DB.
    public async Task GetByZipNoAsync_WhenZipDoesNotExist_ReturnsNull(string nonExistentZipNo)
    // string nonExistentZipNo = "9999"
    {
        // Act
        var result = await _service.GetByZipNoAsync(nonExistentZipNo);
        // var result = await _service.GetByZipNoAsync("9999");

        // Assert
        Assert.Null(result);
    }

    // ==========================================
    // 4. CREATE ZIP TESTS
    // ==========================================

    [Fact]
    public async Task CreateZipAsync_WhenMasterZipDoesNotExist_CreatesNewMasterAndJunction()
    {
        // Arrange
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
        var result = await _service.CreateZipAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("10002", result.ZipNo);
        Assert.Equal("Makati", result.ZipName);

        var savedZipMaster = await _context.Zip.FirstOrDefaultAsync(z => z.ZipNo == "10002");
        Assert.NotNull(savedZipMaster);
    }

    // ==========================================
    // 5. UPDATE ZIP TESTS
    // ==========================================

    [Fact]
    public async Task UpdateZipAsync_WhenZipExists_UpdatesMasterAndReplacesJunction()
    {
        // Arrange
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
        var result = await _service.UpdateZipAsync(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Manila Name", result.ZipName);

        var updatedMasterInDb = await _context.Zip.FirstAsync(z => z.ZipNo == "10001");
        Assert.Equal("Updated Manila Name", updatedMasterInDb.ZipName);
    }

    [Fact]
    public async Task UpdateZipAsync_WhenZipDoesNotExist_ReturnsNull()
    {
        // Arrange
        var updateDto = new ZipUpdateDto 
        { 
            ZipNo = "99999", 
            ZipName = "Non Existent", 
            CountyNo = "C001", 
            ZoNo = "Z101", 
            DoNo = "D001" 
        };

        // Act
        var result = await _service.UpdateZipAsync(updateDto);

        // Assert
        Assert.Null(result);
    }

    // ==========================================
    // 6. DELETE ZIP TESTS
    // ==========================================

    [Theory]
    [MemberData(nameof(DeleteZipIds))]
    public async Task DeleteZipAsync_WhenZipIdExists_ReturnsTrueAndRemovesRecord(int deleteZipIds)
    {
        // Act
        bool result = await _service.DeleteZipAsync(deleteZipIds); // Deletes seeded ZipIds from the DeleteZipIds pre-made data

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Zip.FindAsync(deleteZipIds));
    }

    [Theory]
    [MemberData(nameof(GetInvalidZipIds))]
    public async Task DeleteZipAsync_WhenZipIdDoesNotExist_ReturnsFalse(int invalidZipId)
    {
        // Act
        bool result = await _service.DeleteZipAsync(invalidZipId);

        // Assert
        Assert.False(result);
    }

    // ==========================================
    // 7. EXPORT TO EXCEL TEST
    // ==========================================

    [Fact]
    public async Task ExportZipsToExcelAsync_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var searchDto = new ZipSearchDto();

        // Act
        var excelBytes = await _service.ExportZipsToExcelAsync(searchDto);

        // Assert
        Assert.NotNull(excelBytes);
        Assert.True(excelBytes.Length > 0);
    }

    // ==========================================
    // 8. PROCESS RESUME TESTS
    // ==========================================

    [Fact]
    public async Task ProcessResumeAsync_WhenOriItemNotInVitaeList_ReturnsFalse()
    {
        // Arrange
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
            OriItem = new ZipReadDto { ZipId = 1, ZipNo = "10001" },
            VitaeList = new List<ZipReadDto> { new ZipReadDto { ZipId = 99, ZipNo = "10001" } }
        };

        // Act
        var result = await _service.ProcessResumeAsync(payload);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ProcessResumeAsync_WhenValidResume_ReturnsTrueAndUpdatesDates()
    {
        // Arrange
        DateTime maxDate = DateTime.SpecifyKind(new DateTime(9999, 12, 31), DateTimeKind.Utc);

        var payload = new ZipProcessResumeDto
        {
            NewItem = new ZipCreateDto 
            { 
                ZipNo = "10001", 
                ZipName = "New Manila", 
                CountyNo = "C001", 
                ZoNo = "Z101", 
                DoNo = "D001", 
                EffDateFrom = DateTime.UtcNow 
            },
            OriItem = new ZipReadDto { ZipId = 1, ZipNo = "10001" },
            VitaeList = new List<ZipReadDto> { new ZipReadDto { ZipId = 1, ZipNo = "10001" } }
        };

        // Act
        var result = await _service.ProcessResumeAsync(payload);

        // Assert
        Assert.True(result);

        var updatedActiveZip = await _context.Zip.FindAsync(1);
        Assert.NotNull(updatedActiveZip);
        Assert.NotEqual(maxDate, updatedActiveZip.EffDateTo);
    }

    // ==========================================
    // 9. BUSINESS RULE VALIDATION TESTS (B231002 & B231003)
    // ==========================================

    [Theory]
    [MemberData(nameof(GetForbiddenCharacters))]
    public async Task CreateZipAsync_WhenInputHasForbiddenCharacters_ThrowsB231002(string forbiddenChar)
    {
        // Arrange
        var invalidDto = new ZipCreateDto
        {
            ZipNo = $"10{forbiddenChar}01",
            ZipName = "Manila",
            CountyNo = "C001",
            ZoNo = "Z101",
            DoNo = "D001"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateZipAsync(invalidDto));
        Assert.Contains("B231002", exception.Message);
    }

    [Fact]
    public async Task CreateZipAsync_WhenZipNoExceedsMaxLength_ThrowsB231003()
    {
        // Arrange
        var invalidDto = new ZipCreateDto
        {
            ZipNo = "100001", // 6 characters (> max 5)
            ZipName = "Manila",
            CountyNo = "C001",
            ZoNo = "Z101",
            DoNo = "D001"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateZipAsync(invalidDto));
        Assert.Contains("B231003", exception.Message);
    }

    [Fact]
    public async Task SearchZipsAsync_WhenFilterHasForbiddenCharacters_ThrowsB231002()
    {
        // Arrange
        var searchDto = new ZipSearchDto
        {
            ZipNo = "100^|"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.SearchZipsAsync(searchDto));
        Assert.Contains("B231002", exception.Message);
    }

    [Fact]
    public async Task SearchZipsAsync_WhenCountyNoExceedsMaxLength_ThrowsB231003()
    {
        // Arrange
        var searchDto = new ZipSearchDto
        {
            CountyNo = "111111111" // 9 characters (> max 8)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.SearchZipsAsync(searchDto));
        Assert.Contains("B231003", exception.Message);
    }
}