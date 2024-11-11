using Microsoft.AspNetCore.Mvc;
using Service.Services.Interfaces;

namespace Service.Api.Controllers;

[ApiController]
[Route("faturas-management")]
public class FaturasManagementController(
    IFaturaManagementService _faturaManagementService
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _faturaManagementService.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("expensives-items")]
    public async Task<IActionResult> Get10ItemsMoreExpensive()
    {
        var result = await _faturaManagementService.Get10ItemsMoreExpensiveAsync();

        return Ok(result);
    }

    [HttpGet("expensives-faturas")]
    public async Task<IActionResult> Get10FaturasMoreExpensive()
    {
        var result = await _faturaManagementService.Get10FaturasMoreExpensiveAsync();

        return Ok(result);
    }

    [HttpGet("total/{client}")]
    public async Task<IActionResult> TotalByClient([FromRoute] string client)
    {
        var result = await _faturaManagementService.GetTotalByClientAsync(client);

        return Ok(result);
    }

    [HttpGet("total/{year}/{month}")]
    public async Task<IActionResult> TotalByMonthAndyear([FromRoute] int year, [FromRoute] int month)
    {
        var result = await _faturaManagementService.GetTotalByYearAndMonthAsync(year, month);

        return Ok(result);
    }
}
