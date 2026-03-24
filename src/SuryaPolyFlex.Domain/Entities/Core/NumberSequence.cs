using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class NumberSequence : BaseEntity
{
    public string Prefix { get; set; } = default!;      // e.g. IND, PO, GRN, SO
    public string ModuleCode { get; set; } = default!;   // e.g. INDENT, PO, GRN
    public int LastNumber { get; set; } = 0;
    public int PaddingLength { get; set; } = 5;          // e.g. IND-00001
    public string? FinancialYear { get; set; }            // e.g. 2425
}