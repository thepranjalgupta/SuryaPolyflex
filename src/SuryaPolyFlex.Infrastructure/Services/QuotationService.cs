using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.Quotations;
using SuryaPolyFlex.Domain.Entities.Sales;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class QuotationService : IQuotationService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;

    public QuotationService(AppDbContext context, INumberSequenceService numberService)
    {
        _context       = context;
        _numberService = numberService;
    }

    public async Task<List<QuotationDto>> GetAllAsync(string? status = null)
    {
        var query = _context.Quotations
            .Include(q => q.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<QuotationStatus>(status, out var parsed))
            query = query.Where(q => q.Status == parsed);

        return await query
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuotationDto
            {
                Id              = q.Id,
                QuotationNumber = q.QuotationNumber,
                QuotationDate   = q.QuotationDate,
                CustomerId      = q.CustomerId,
                CustomerName    = q.Customer.Name,
                LeadId          = q.LeadId,
                Status          = q.Status.ToString(),
                ValidUntil      = q.ValidUntil,
                Terms           = q.Terms,
                Remarks         = q.Remarks,
                TotalAmount     = q.TotalAmount,
                Revision        = q.Revision
            }).ToListAsync();
    }

    public async Task<QuotationDto?> GetByIdAsync(int id)
    {
        var q = await _context.Quotations
            .Include(q => q.Customer)
            .Include(q => q.Items)
                .ThenInclude(i => i.Item)
            .Include(q => q.Items)
                .ThenInclude(i => i.UoM)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (q == null) return null;

        return new QuotationDto
        {
            Id              = q.Id,
            QuotationNumber = q.QuotationNumber,
            QuotationDate   = q.QuotationDate,
            CustomerId      = q.CustomerId,
            CustomerName    = q.Customer.Name,
            LeadId          = q.LeadId,
            Status          = q.Status.ToString(),
            ValidUntil      = q.ValidUntil,
            Terms           = q.Terms,
            Remarks         = q.Remarks,
            TotalAmount     = q.TotalAmount,
            Revision        = q.Revision,
            Items           = q.Items.Select(i => new QuotationItemDto
            {
                Id          = i.Id,
                ItemId      = i.ItemId,
                ItemCode    = i.Item?.ItemCode ?? "",
                Description = i.Description,
                Qty         = i.Qty,
                UoMCode     = i.UoM?.Code,
                UnitRate    = i.UnitRate,
                DiscountPct = i.DiscountPct,
                TaxPct      = i.TaxPct,
                LineTotal   = i.Qty * i.UnitRate * (1 - i.DiscountPct / 100)
            }).ToList()
        };
    }

    public async Task<(bool Success, string Message, int QuotationId)> CreateAsync(
        CreateQuotationDto dto, string createdBy)
    {
        if (!dto.Items.Any())
            return (false, "Add at least one item.", 0);

        var qtnNumber   = await _numberService.GenerateAsync("QTN");
        var totalAmount = dto.Items.Sum(i => i.Qty * i.UnitRate * (1 - i.DiscountPct / 100));

        var quotation = new Quotation
        {
            QuotationNumber = qtnNumber,
            QuotationDate   = DateTime.UtcNow,
            CustomerId      = dto.CustomerId,
            LeadId          = dto.LeadId,
            Status          = QuotationStatus.Draft,
            ValidUntil      = dto.ValidUntil,
            Terms           = dto.Terms,
            Remarks         = dto.Remarks,
            TotalAmount     = totalAmount,
            Revision        = 1,
            CreatedBy       = createdBy,
            CreatedAt       = DateTime.UtcNow,
            Items = dto.Items.Select(i => new QuotationItem
            {
                ItemId      = i.ItemId,
                Description = i.Description,
                Qty         = i.Qty,
                UoMId       = i.UoMId,
                UnitRate    = i.UnitRate,
                DiscountPct = i.DiscountPct,
                TaxPct      = i.TaxPct,
                CreatedBy   = createdBy,
                CreatedAt   = DateTime.UtcNow
            }).ToList()
        };

        _context.Quotations.Add(quotation);
        await _context.SaveChangesAsync();
        return (true, $"Quotation {qtnNumber} created.", quotation.Id);
    }

    public async Task<(bool Success, string Message)> UpdateStatusAsync(
        int id, string status, string updatedBy)
    {
        var q = await _context.Quotations.FindAsync(id);
        if (q == null) return (false, "Quotation not found.");

        if (!Enum.TryParse<QuotationStatus>(status, out var parsed))
            return (false, "Invalid status.");

        q.Status    = parsed;
        q.UpdatedBy = updatedBy;
        q.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, $"Quotation marked as {status}.");
    }
}