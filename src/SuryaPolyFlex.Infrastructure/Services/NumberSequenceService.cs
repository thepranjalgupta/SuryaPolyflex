using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class NumberSequenceService : INumberSequenceService
{
    private readonly AppDbContext _context;

    public NumberSequenceService(AppDbContext context) => _context = context;

    public async Task<string> GenerateAsync(string moduleCode)
    {
        // Use Indian Standard Time for FY calculation
        var ist     = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
        var nowIst  = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ist);

        // Indian FY: April to March
        // If month >= April → FY is thisYear/nextYear e.g. April 2025 → "2526"
        // If month < April  → FY is lastYear/thisYear e.g. Jan 2026   → "2526"
        var fy = nowIst.Month >= 4
            ? $"{nowIst.Year % 100}{(nowIst.Year + 1) % 100}"
            : $"{(nowIst.Year - 1) % 100}{nowIst.Year % 100}";

        var seq = await _context.NumberSequences
            .Where(n => n.ModuleCode == moduleCode && n.FinancialYear == fy)
            .FirstOrDefaultAsync();

        // Auto-create sequence if missing — handles FY rollover gracefully
        if (seq == null)
        {
            var prefix = moduleCode switch
            {
                "INDENT" => "IND",
                "PO"     => "PO",
                "GRN"    => "GRN",
                "SO"     => "SO",
                "JC"     => "JC",
                "WO"     => "WO",
                "DC"     => "DC",
                "GATE"   => "GATE",
                "QTN"    => "QTN",
                _        => moduleCode
            };

            seq = new NumberSequence
            {
                ModuleCode    = moduleCode,
                Prefix        = prefix,
                LastNumber    = 0,
                PaddingLength = 5,
                FinancialYear = fy,
                CreatedBy     = "SYSTEM",
                CreatedAt     = DateTime.UtcNow
            };

            _context.NumberSequences.Add(seq);
            await _context.SaveChangesAsync();
        }

        seq.LastNumber++;
        await _context.SaveChangesAsync();

        var paddedNumber = seq.LastNumber
            .ToString()
            .PadLeft(seq.PaddingLength, '0');

        return $"{seq.Prefix}-{fy}-{paddedNumber}";
    }
}