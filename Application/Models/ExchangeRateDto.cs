namespace Application.Models;

public sealed class ExchangeRateDto
{
    public string Moneda { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Compra { get; set; }
    public decimal Venta { get; set; }
    public DateTimeOffset FechaActualizacion { get; set; }
}
