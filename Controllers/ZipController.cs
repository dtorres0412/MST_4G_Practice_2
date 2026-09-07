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
}