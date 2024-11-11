using Microsoft.AspNetCore.Mvc;
using Service.Entities.Requests;
using Service.Services.Interfaces;

namespace Service.Api.Controllers;

[ApiController]
[Route("faturas")]
public class FaturasController(
    IFaturaService _faturaService
) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var result = await _faturaService.GetByIdasync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FaturaCreateRequest request)
    {
        await _faturaService.Createasync(request);

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Close([FromRoute] int id)
    {
        await _faturaService.CloseAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _faturaService.DeleteAynsc(id);

        return NoContent();
    }
}
