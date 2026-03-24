using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Items;

public class ItemDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public int UoMId { get; set; }
    public string UoMCode { get; set; } = default!;
    public string ItemType { get; set; } = default!;
    public decimal MinStockLevel { get; set; }
    public decimal ReorderQty { get; set; }
    public decimal StandardCost { get; set; }
    public bool IsActive { get; set; }
}

public class CreateItemDto
{
    [Required] public string ItemCode { get; set; } = default!;
    [Required] public string Name { get; set; } = default!;
    public string? Description { get; set; }
    [Required] public int CategoryId { get; set; }
    [Required] public int UoMId { get; set; }
    [Required] public string ItemType { get; set; } = "RM";
    public decimal MinStockLevel { get; set; } = 0;
    public decimal ReorderQty { get; set; } = 0;
    public decimal StandardCost { get; set; } = 0;
}

public class EditItemDto : CreateItemDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}

public class ItemSelectDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string UoMCode { get; set; } = default!;
}