using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.SalesOrders;

public class SalesOrderDto
{
    public int Id { get; set; }
    public string SONumber { get; set; } = default!;
    public DateTime SODate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public int? QuotationId { get; set; }
    public string? QuotationNumber { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? RequiredByDate { get; set; }
    public string? POReference { get; set; }
    public string? Remarks { get; set; }
    public decimal TotalAmount { get; set; }
    public List<SOItemDto> Items { get; set; } = new();
    public List<CustomerJobDto> CustomerJobs { get; set; } = new();
}

public class SOItemDto
{
    public int Id { get; set; }
    public int? ItemId { get; set; }
    public string Description { get; set; } = default!;
    public decimal OrderedQty { get; set; }
    public decimal DispatchedQty { get; set; }
    public decimal PendingQty { get; set; }
    public string? UoMCode { get; set; }
    public decimal UnitRate { get; set; }
    public decimal LineTotal { get; set; }
}

public class CustomerJobDto
{
    public int Id { get; set; }
    public string JobTitle { get; set; } = default!;
    public string? Substrate { get; set; }
    public decimal? Width { get; set; }
    public decimal? Length { get; set; }
    public int? ColorCount { get; set; }
    public string? Finish { get; set; }
    public decimal Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
    public string Status { get; set; } = default!;
}

public class CreateSODto
{
    [Required] public int CustomerId { get; set; }
    public int? QuotationId { get; set; }
    public DateTime? RequiredByDate { get; set; }
    public string? POReference { get; set; }
    public string? Remarks { get; set; }

    [Required] public List<CreateSOItemDto> Items { get; set; } = new();
}

public class CreateSOItemDto
{
    public int? ItemId { get; set; }
    [Required] public string Description { get; set; } = default!;
    [Required, Range(0.001, double.MaxValue)] public decimal OrderedQty { get; set; }
    public int? UoMId { get; set; }
    [Required, Range(0.01, double.MaxValue)]  public decimal UnitRate { get; set; }
    public decimal TaxPct { get; set; } = 0;
}

public class CreateCustomerJobDto
{
    public int SalesOrderId { get; set; }
    [Required] public string JobTitle { get; set; } = default!;
    public string? Substrate { get; set; }
    public decimal? Width { get; set; }
    public decimal? Length { get; set; }
    public int? ColorCount { get; set; }
    public string? Finish { get; set; }
    [Required] public decimal Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}