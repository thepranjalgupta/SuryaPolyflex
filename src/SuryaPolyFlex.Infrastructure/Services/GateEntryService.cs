using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.GateEntry;
using SuryaPolyFlex.Domain.Entities.Procurement;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class GateEntryService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;

    public GateEntryService(AppDbContext context, INumberSequenceService numberService)
    {
        _context       = context;
        _numberService = numberService;
    }

    public async Task<List<GateEntryDto>> GetAllAsync()
    {
        return await _context.GateEntries
            .Include(g => g.Vendor)
            .Include(g => g.PurchaseOrder)
            .OrderByDescending(g => g.EntryDateTime)
            .Select(g => new GateEntryDto
            {
                Id            = g.Id,
                GateEntryNo   = g.GateEntryNo,
                VendorName    = g.Vendor.Name,
                PONumber      = g.PurchaseOrder != null
                    ? g.PurchaseOrder.PONumber
                    : (g.POId.HasValue ? _context.PurchaseOrders
                        .Where(p => p.Id == g.POId.Value)
                        .Select(p => p.PONumber)
                        .FirstOrDefault() : null),
                VehicleNumber = g.VehicleNumber,
                DriverName    = g.DriverName,
                EntryDateTime = g.EntryDateTime,
                ExitDateTime  = g.ExitDateTime,
                ReceivedBy    = g.ReceivedBy,
                Remarks       = g.Remarks,
                GRNCreated    = g.GRNId.HasValue
            }).ToListAsync();
    }

    public async Task<(bool Success, string Message, int Id)> CreateAsync(
        CreateGateEntryDto dto, string createdBy)
    {
        var gateNo = await _numberService.GenerateAsync("GATE");

        _context.GateEntries.Add(new GateEntry
        {
            GateEntryNo   = gateNo,
            VendorId      = dto.VendorId,
            POId          = dto.POId,
            VehicleNumber = dto.VehicleNumber,
            DriverName    = dto.DriverName,
            EntryDateTime = dto.EntryDateTime.ToUniversalTime(),
            ReceivedBy    = dto.ReceivedBy,
            Remarks       = dto.Remarks,
            CreatedBy     = createdBy,
            CreatedAt     = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, $"Gate Entry {gateNo} recorded.", 0);
    }

    public async Task<(bool Success, string Message)> MarkExitAsync(int id)
    {
        var entry = await _context.GateEntries.FindAsync(id);
        if (entry == null) return (false, "Gate entry not found.");

        entry.ExitDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Exit time recorded.");
    }
}