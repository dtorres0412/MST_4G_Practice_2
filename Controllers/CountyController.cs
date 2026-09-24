using Microsoft.AspNetCore.Mvc;
using MST_4G.Services;
using MST_4G.Dtos;

namespace MST_4G.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountyController(ICountyService countyService) : ControllerBase
{
     [HttpGet("{countyNo}")]
    public async Task<ActionResult<CountyReadDto>> GetByCountyNo(int countyNo)
    {
        var County = await countyService.GetByCountyNoAsync(countyNo);

        if (County == null)
            return NotFound($"County record with CountyNo '{countyNo}' not found.");

        return Ok(County);
    }

    [HttpPost("create")]
    public async Task<ActionResult<CountyReadDto>> CreateCounty([FromBody] CountyCreateDto createCountyDto)
    {
        try
        {
            var result = await countyService.CreateCountyAsync(createCountyDto);

            return CreatedAtAction(nameof(GetByCountyNo), new {countyNo = result?.CountyNo}, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

    }
    
}
