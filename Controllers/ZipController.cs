using Microsoft.AspNetCore.Mvc;
using MST_4G.Services;
using MST_4G.Dtos;

namespace MST_4G.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZipController(IZipService zipService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ZipReadDto>>> SearchZips([FromQuery] ZipSearchDto searchDto)
    {
        var result = await zipService.SearchZipsAsync(searchDto);
        return Ok(result);
    }

    [HttpGet("{zipNo}")]
    public async Task<ActionResult<ZipReadDto>> GetByZipNo(string zipNo)
    {
        var zip = await zipService.GetByZipNoAsync(zipNo);

        if (zip == null)
            return NotFound($"Zip record with ZipNo '{zipNo}' not found.");

        return Ok(zip);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportZips([FromQuery] ZipSearchDto searchDto)
    {
        var fileBytes = await zipService.ExportZipsToExcelAsync(searchDto);
        var fileName = $"Zip_Maintenance_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost("create")]
    public async Task<ActionResult<ZipReadDto>> CreateZip([FromBody] ZipCreateDto createDto)
    {
        try
        {
            var result = await zipService.CreateZipAsync(createDto);

            return CreatedAtAction(nameof(GetByZipNo), new { zipNo = result?.ZipNo }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update")]
    public async Task<ActionResult<ZipReadDto>> UpdateZip([FromBody] ZipUpdateDto updateDto)
    {
        try
        {
            var result = await zipService.UpdateZipAsync(updateDto);

            if (result == null)
                return NotFound($"Zip record with ZipNo '{updateDto.ZipNo}' not found.");

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}