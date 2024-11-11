using Microsoft.AspNetCore.Mvc;
using Service.Entities.Requests;
using Service.Services.Interfaces;

namespace Service.Api.Controllers;

[ApiController]
[Route("faturas/{faturaId}/items")]
public class FaturaItemController(
    IFaturaItemService _faturaItemservice
) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddToFatura([FromRoute] int faturaId, [FromBody] FaturaItemAddToFaturaRequest request)
    {
        await _faturaItemservice.AddToFaturaAsync(faturaId, request);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveToFatura([FromRoute] int faturaId, [FromRoute] int id)
    {
        await _faturaItemservice.RemoveToFaturaAsync(id, faturaId);

        return NoContent();
    }
}
