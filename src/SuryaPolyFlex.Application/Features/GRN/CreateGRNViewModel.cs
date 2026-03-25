namespace SuryaPolyFlex.Application.Features.GRN;

public class CreateGRNViewModel
{
    public CreateGRNDto Form { get; set; } = new();
    public List<GRNLineItem> Lines { get; set; } = new();
}

public class GRNLineItem
{
    public int    POItemId    { get; set; }
    public int    ItemId      { get; set; }
    public string ItemCode    { get; set; } = default!;
    public string ItemName    { get; set; } = default!;
    public string UoMCode     { get; set; } = default!;
    public decimal PendingQty { get; set; }
    public decimal UnitCost   { get; set; }
}