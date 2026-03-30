using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class CustomerJob : BaseEntity
{
    public int SalesOrderId { get; set; }
    public string JobTitle { get; set; } = default!;
    public string? Substrate { get; set; }
    public decimal? Width { get; set; }
    public decimal? Length { get; set; }
    public int? ColorCount { get; set; }
    public string? Finish { get; set; }
    public decimal Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
    public string Status { get; set; } = "Pending";

    public SalesOrder SalesOrder { get; set; } = default!;
}