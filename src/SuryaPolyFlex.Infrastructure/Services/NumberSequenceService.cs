using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class NumberSequenceService : INumberSequenceService
{
    private readonly AppDbContext _context;

    public NumberSequenceService(AppDbContext context) => _context = context;

    public async Task<string> GenerateAsync(string moduleCode)
    {
        // Get current financial year e.g. 2425
        var now = DateTime.Now;
        var fy  = now.Month >= 4
            ? $"{now.Year % 100}{(now.Year + 1) % 100}"
            : $"{(now.Year - 1) % 100}{now.Year % 100}";

        // Use row-level lock to prevent duplicate numbers
        var seq = await _context.NumberSequences
            .Where(n => n.ModuleCode == moduleCode && n.FinancialYear == fy)
            .FirstOrDefaultAsync();

        if (seq == null)
            throw new InvalidOperationException(
                $"Number sequence not configured for module '{moduleCode}' FY '{fy}'.");

        seq.LastNumber++;
        await _context.SaveChangesAsync();

        // Format: IND-2425-00001
        var paddedNumber = seq.LastNumber.ToString()
            .PadLeft(seq.PaddingLength, '0');

        return $"{seq.Prefix}-{fy}-{paddedNumber}";
    }
}