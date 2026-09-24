using Microsoft.AspNetCore.Mvc;
using MST_4G.Services;
using MST_4G.Dtos;

namespace MST_4G.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZoController(IZoService zoService) : ControllerBase
{
     [HttpGet("{zoNo}")]
    public async Task<ActionResult<ZoReadDto>> GetByZoNo(string zoNo)
    {
        var Zo = await zoService.GetByZoNoAsync(zoNo);

        if (Zo == null)
            return NotFound($"County record with ZoNo '{zoNo}' not found.");

        return Ok(Zo);
    }

    [HttpPost("create")]
    public async Task<ActionResult<ZoReadDto>> ZoCounty([FromBody] ZoCreateDto zoCreateDto)
    {
        try
        {
            var result = await zoService.CreateZoAsync(zoCreateDto);

            return CreatedAtAction(nameof(GetByZoNo), new {zoNo = result?.ZoNo}, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

    }
    
}
