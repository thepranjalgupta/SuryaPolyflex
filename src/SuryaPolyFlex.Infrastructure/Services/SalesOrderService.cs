using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.SalesOrders;
using SuryaPolyFlex.Domain.Entities.Sales;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class SalesOrderService : ISalesOrderService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;

    public SalesOrderService(AppDbContext context, INumberSequenceService numberService)
    {
        _context       = context;
        _numberService = numberService;
    }

    public async Task<List<SalesOrderDto>> GetAllAsync(string? status = null)
    {
        var query = _context.SalesOrders
            .Include(s => s.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<SalesOrderStatus>(status, out var parsed))
            query = query.Where(s => s.Status == parsed);

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SalesOrderDto
            {
                Id           = s.Id,
                SONumber     = s.SONumber,
                SODate       = s.SODate,
                CustomerId   = s.CustomerId,
                CustomerName = s.Customer.Name,
                QuotationId  = s.QuotationId,
                Status       = s.Status.ToString(),
                RequiredByDate = s.RequiredByDate,
                POReference  = s.POReference,
                Remarks      = s.Remarks,
                TotalAmount  = s.TotalAmount
            }).ToListAsync();
    }

    public async Task<SalesOrderDto?> GetByIdAsync(int id)
    {
        var so = await _context.SalesOrders
            .Include(s => s.Customer)
            .Include(s => s.Quotation)
            .Include(s => s.Items)
                .ThenInclude(i => i.UoM)
            .Include(s => s.CustomerJobs)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (so == null) return null;

        return new SalesOrderDto
        {
            Id              = so.Id,
            SONumber        = so.SONumber,
            SODate          = so.SODate,
            CustomerId      = so.CustomerId,
            CustomerName    = so.Customer.Name,
            QuotationId     = so.QuotationId,
            QuotationNumber = so.Quotation?.QuotationNumber,
            Status          = so.Status.ToString(),
            RequiredByDate  = so.RequiredByDate,
            POReference     = so.POReference,
            Remarks         = so.Remarks,
            TotalAmount     = so.TotalAmount,
            Items = so.Items.Select(i => new SOItemDto
            {
                Id           = i.Id,
                ItemId       = i.ItemId,
                Description  = i.Description,
                OrderedQty   = i.OrderedQty,
                DispatchedQty = i.DispatchedQty,
                PendingQty   = i.OrderedQty - i.DispatchedQty,
                UoMCode      = i.UoM?.Code,
                UnitRate     = i.UnitRate,
                LineTotal    = i.OrderedQty * i.UnitRate
            }).ToList(),
            CustomerJobs = so.CustomerJobs.Select(j => new CustomerJobDto
            {
                Id                  = j.Id,
                JobTitle            = j.JobTitle,
                Substrate           = j.Substrate,
                Width               = j.Width,
                Length              = j.Length,
                ColorCount          = j.ColorCount,
                Finish              = j.Finish,
                Quantity            = j.Quantity,
                SpecialInstructions = j.SpecialInstructions,
                Status              = j.Status
            }).ToList()
        };
    }

    public async Task<(bool Success, string Message, int SOId)> CreateAsync(
        CreateSODto dto, string createdBy)
    {
        if (!dto.Items.Any())
            return (false, "Add at least one item.", 0);

        var soNumber    = await _numberService.GenerateAsync("SO");
        var totalAmount = dto.Items.Sum(i => i.OrderedQty * i.UnitRate);

        var so = new SalesOrder
        {
            SONumber       = soNumber,
            SODate         = DateTime.UtcNow,
            CustomerId     = dto.CustomerId,
            QuotationId    = dto.QuotationId,
            Status         = SalesOrderStatus.Open,
            RequiredByDate = dto.RequiredByDate,
            POReference    = dto.POReference,
            Remarks        = dto.Remarks,
            TotalAmount    = totalAmount,
            CreatedBy      = createdBy,
            CreatedAt      = DateTime.UtcNow,
            Items = dto.Items.Select(i => new SOItem
            {
                ItemId      = i.ItemId,
                Description = i.Description,
                OrderedQty  = i.OrderedQty,
                UoMId       = i.UoMId,
                UnitRate    = i.UnitRate,
                TaxPct      = i.TaxPct,
                CreatedBy   = createdBy,
                CreatedAt   = DateTime.UtcNow
            }).ToList()
        };

        _context.SalesOrders.Add(so);
        await _context.SaveChangesAsync();

        // Mark quotation as accepted if linked
        if (dto.QuotationId.HasValue)
        {
            var qtn = await _context.Quotations.FindAsync(dto.QuotationId.Value);
            if (qtn != null)
            {
                qtn.Status    = Domain.Enums.QuotationStatus.Accepted;
                qtn.UpdatedAt = DateTime.UtcNow;
                qtn.UpdatedBy = createdBy;
                await _context.SaveChangesAsync();
            }
        }

        return (true, $"Sales Order {soNumber} created.", so.Id);
    }

    public async Task<(bool Success, string Message)> UpdateStatusAsync(
        int id, string status, string updatedBy)
    {
        var so = await _context.SalesOrders.FindAsync(id);
        if (so == null) return (false, "Sales Order not found.");

        if (!Enum.TryParse<SalesOrderStatus>(status, out var parsed))
            return (false, "Invalid status.");

        so.Status    = parsed;
        so.UpdatedBy = updatedBy;
        so.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, $"Status updated to {status}.");
    }

    public async Task<(bool Success, string Message)> AddCustomerJobAsync(
        CreateCustomerJobDto dto, string createdBy)
    {
        var so = await _context.SalesOrders.FindAsync(dto.SalesOrderId);
        if (so == null) return (false, "Sales Order not found.");

        _context.CustomerJobs.Add(new CustomerJob
        {
            SalesOrderId        = dto.SalesOrderId,
            JobTitle            = dto.JobTitle,
            Substrate           = dto.Substrate,
            Width               = dto.Width,
            Length              = dto.Length,
            ColorCount          = dto.ColorCount,
            Finish              = dto.Finish,
            Quantity            = dto.Quantity,
            SpecialInstructions = dto.SpecialInstructions,
            Status              = "Pending",
            CreatedBy           = createdBy,
            CreatedAt           = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Customer job added.");
    }
}