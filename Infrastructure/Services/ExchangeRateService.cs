using System.Net.Http.Json;
using Application.Interfaces;
using Application.Models;

namespace Infrastructure.Services;

public sealed class ExchangeRateService(HttpClient httpClient) : IExchangeRateService
{
    public async Task<ExchangeRateDto> GetOfficialUsdAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("v1/dolares/oficial", cancellationToken);
        response.EnsureSuccessStatusCode();

        var exchangeRate = await response.Content.ReadFromJsonAsync<ExchangeRateDto>(
            cancellationToken: cancellationToken);

        if (exchangeRate is null || exchangeRate.Venta <= 0)
            throw new HttpRequestException("La API externa devolvió una cotización inválida.");

        return exchangeRate;
    }
}
