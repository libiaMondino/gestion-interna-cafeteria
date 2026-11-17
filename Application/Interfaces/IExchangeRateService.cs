using Application.Models;

namespace Application.Interfaces;

public interface IExchangeRateService
{
    Task<ExchangeRateDto> GetOfficialUsdAsync(CancellationToken cancellationToken = default);
}
