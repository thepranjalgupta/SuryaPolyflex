using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Warehouses;

public class WarehouseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public string WarehouseType { get; set; } = default!;
    public bool IsActive { get; set; }
}

public class CreateWarehouseDto
{
    [Required] public string Code { get; set; } = default!;
    [Required] public string Name { get; set; } = default!;
    public string? Location { get; set; }
    [Required] public string WarehouseType { get; set; } = "RM";
}

public class EditWarehouseDto : CreateWarehouseDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}