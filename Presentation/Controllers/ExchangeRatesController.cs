using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/exchange-rates")]
[Authorize(Roles = "Admin")]
public sealed class ExchangeRatesController(IExchangeRateService exchangeRateService) : ControllerBase
{
    [HttpGet("usd")]
    public async Task<IActionResult> GetOfficialUsd(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await exchangeRateService.GetOfficialUsdAsync(cancellationToken));
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(503, new { error = "Tiempo de espera agotado al consultar el servicio externo." });
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new { error = "No se pudo consultar el servicio externo." });
        }
        catch (System.Text.Json.JsonException)
        {
            return StatusCode(503, new { error = "El servicio externo devolvió una respuesta inválida." });
        }
    }
}
