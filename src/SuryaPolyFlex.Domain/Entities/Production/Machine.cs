using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class Machine : BaseEntity
{
    public string MachineCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Type { get; set; }
    public bool IsActive { get; set; } = true;
}