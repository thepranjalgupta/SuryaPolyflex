using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.MaterialIssue;
using SuryaPolyFlex.Domain.Entities.Procurement;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class MaterialIssueService : IMaterialIssueService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;
    private readonly IStockService _stockService;

    public MaterialIssueService(
        AppDbContext context,
        INumberSequenceService numberService,
        IStockService stockService)
    {
        _context       = context;
        _numberService = numberService;
        _stockService  = stockService;
    }

    public async Task<List<MaterialIssueDto>> GetAllAsync()
    {
        return await _context.MaterialIssues
            .Include(m => m.Items)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MaterialIssueDto
            {
                Id          = m.Id,
                IssueNo     = m.IssueNo,
                IssueDate   = m.IssueDate,
                Remarks     = m.Remarks
            }).ToListAsync();
    }

    public async Task<MaterialIssueDto?> GetByIdAsync(int id)
    {
        var issue = await _context.MaterialIssues
            .Include(m => m.Items)
                .ThenInclude(i => i.Item)
                    .ThenInclude(i => i!.UoM)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (issue == null) return null;

        return new MaterialIssueDto
        {
            Id        = issue.Id,
            IssueNo   = issue.IssueNo,
            IssueDate = issue.IssueDate,
            Remarks   = issue.Remarks,
            Items = issue.Items.Select(i => new MaterialIssueItemDto
            {
                ItemId       = i.ItemId,
                ItemCode     = i.Item?.ItemCode ?? "",
                ItemName     = i.Item?.Name ?? "",
                UoMCode      = i.Item?.UoM?.Code,
                RequestedQty = i.RequestedQty,
                IssuedQty    = i.IssuedQty
            }).ToList()
        };
    }

    public async Task<(bool Success, string Message, int Id)> CreateAsync(
        CreateMaterialIssueDto dto, string createdBy)
    {
        if (!dto.Items.Any())
            return (false, "Add at least one item.", 0);

        // Validate stock for each item
        foreach (var item in dto.Items)
        {
            var balance = await _stockService.GetBalanceAsync(
                item.ItemId, dto.FromWarehouseId);
            if (balance < item.IssuedQty)
                return (false, $"Insufficient stock for item ID {item.ItemId}. Available: {balance}", 0);
        }

        var issueNo = await _numberService.GenerateAsync("MIS");

        var issue = new MaterialIssue
        {
            IssueNo         = issueNo,
            IssueDate       = DateTime.UtcNow,
            FromWarehouseId = dto.FromWarehouseId,
            ToDepartmentId  = dto.ToDepartmentId,
            WorkOrderId     = dto.WorkOrderId,
            IssuedById      = createdBy,
            Remarks         = dto.Remarks,
            CreatedBy       = createdBy,
            CreatedAt       = DateTime.UtcNow,
            Items = dto.Items.Select(i => new MaterialIssueItem
            {
                ItemId       = i.ItemId,
                RequestedQty = i.RequestedQty,
                IssuedQty    = i.IssuedQty,
                UoMId        = i.UoMId,
                CreatedBy    = createdBy,
                CreatedAt    = DateTime.UtcNow
            }).ToList()
        };

        _context.MaterialIssues.Add(issue);
        await _context.SaveChangesAsync();

        // Deduct stock
        foreach (var item in dto.Items)
        {
            await _stockService.IssueStockAsync(
                itemId:          item.ItemId,
                warehouseId:     dto.FromWarehouseId,
                qty:             item.IssuedQty,
                transactionType: StockTransactionType.MaterialIssue,
                referenceType:   "MaterialIssue",
                referenceId:     issue.Id,
                referenceNumber: issue.IssueNo,
                createdBy:       createdBy
            );
        }

        return (true, $"Material Issue {issueNo} created.", issue.Id);
    }
}