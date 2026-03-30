using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.Production;
using SuryaPolyFlex.Application.Features.SalesOrders;
using SuryaPolyFlex.Domain.Entities.Production;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class ProductionService : IProductionService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;
    private readonly IStockService _stockService;

    public ProductionService(
        AppDbContext context,
        INumberSequenceService numberService,
        IStockService stockService)
    {
        _context       = context;
        _numberService = numberService;
        _stockService  = stockService;
    }

    // ── MACHINES ─────────────────────────────────────────────────────────
    public async Task<List<MachineDto>> GetMachinesAsync()
    {
        return await _context.Machines
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.Name)
            .Select(m => new MachineDto
            {
                Id          = m.Id,
                MachineCode = m.MachineCode,
                Name        = m.Name,
                Type        = m.Type,
                IsActive    = m.IsActive
            }).ToListAsync();
    }

    public async Task<(bool Success, string Message)> CreateMachineAsync(
        CreateMachineDto dto, string createdBy)
    {
        if (await _context.Machines.AnyAsync(m => m.MachineCode == dto.MachineCode.ToUpper()))
            return (false, $"Machine code '{dto.MachineCode}' already exists.");

        _context.Machines.Add(new Machine
        {
            MachineCode = dto.MachineCode.ToUpper().Trim(),
            Name        = dto.Name.Trim(),
            Type        = dto.Type?.Trim(),
            IsActive    = true,
            CreatedBy   = createdBy,
            CreatedAt   = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return (true, "Machine created.");
    }

    // ── JOB CARDS ─────────────────────────────────────────────────────────
    public async Task<List<JobCardDto>> GetJobCardsAsync(string? status = null)
    {
        var query = _context.JobCards
            .Include(j => j.Machine)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<JobCardStatus>(status, out var parsed))
            query = query.Where(j => j.Status == parsed);

        var cards = await query.OrderByDescending(j => j.CreatedAt).ToListAsync();

        var soIds = cards.Where(j => j.SOId.HasValue).Select(j => j.SOId!.Value).Distinct().ToList();
        var soDict = await _context.SalesOrders
            .Include(s => s.Customer)
            .Where(s => soIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id);

        return cards.Select(j => new JobCardDto
        {
            Id               = j.Id,
            JobCardNo        = j.JobCardNo,
            SOId             = j.SOId,
            SONumber         = j.SOId.HasValue ? soDict.GetValueOrDefault(j.SOId.Value)?.SONumber : null,
            SODate           = j.SOId.HasValue ? soDict.GetValueOrDefault(j.SOId.Value)?.SODate : null,
            SORequiredBy     = j.SOId.HasValue ? soDict.GetValueOrDefault(j.SOId.Value)?.RequiredByDate : null,
            SOCustomerName   = j.SOId.HasValue ? soDict.GetValueOrDefault(j.SOId.Value)?.Customer?.Name : null,
            SOStatus         = j.SOId.HasValue ? soDict.GetValueOrDefault(j.SOId.Value)?.Status.ToString() : null,
            CustomerJobId    = j.CustomerJobId,
            PlannedStartDate = j.PlannedStartDate,
            PlannedEndDate   = j.PlannedEndDate,
            MachineName      = j.Machine?.Name,
            Shift            = j.Shift,
            TargetQty        = j.TargetQty,
            Status           = j.Status.ToString(),
            Remarks          = j.Remarks
        }).ToList();
    }

    public async Task<JobCardDto?> GetJobCardByIdAsync(int id)
    {
        var jc = await _context.JobCards
            .Include(j => j.Machine)
            .Include(j => j.WorkOrders)
                .ThenInclude(w => w.Machine)
            .Include(j => j.WorkOrders)
                .ThenInclude(w => w.ProductionEntries)
            .Include(j => j.BOMs)
                .ThenInclude(b => b.Items)
                    .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jc == null) return null;

        var so = jc.SOId.HasValue
            ? await _context.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.Items)
                .Include(s => s.CustomerJobs)
                .FirstOrDefaultAsync(s => s.Id == jc.SOId.Value)
            : null;

        return new JobCardDto
        {
            Id               = jc.Id,
            JobCardNo        = jc.JobCardNo,
            SOId             = jc.SOId,
            SONumber         = so?.SONumber,
            SODate           = so?.SODate,
            SORequiredBy     = so?.RequiredByDate,
            SOCustomerName   = so?.Customer?.Name,
            SOStatus         = so?.Status.ToString(),
            SOItems          = so?.Items.Select(i => new SOItemDto
            {
                Id = i.Id,
                ItemId = i.ItemId,
                Description = i.Description,
                OrderedQty = i.OrderedQty,
                DispatchedQty = i.DispatchedQty,
                PendingQty = i.PendingQty,
                UoMCode = i.UoM?.Code,
                UnitRate = i.UnitRate,
                LineTotal = i.LineTotal
            }).ToList() ?? new List<SOItemDto>(),
            CustomerJobs     = so?.CustomerJobs.Select(j => new CustomerJobDto
            {
                Id = j.Id,
                JobTitle = j.JobTitle,
                Substrate = j.Substrate,
                Width = j.Width,
                Length = j.Length,
                ColorCount = j.ColorCount,
                Finish = j.Finish,
                Quantity = j.Quantity,
                SpecialInstructions = j.SpecialInstructions,
                Status = j.Status.ToString()
            }).ToList() ?? new List<CustomerJobDto>(),
            CustomerJobId    = jc.CustomerJobId,
            PlannedStartDate = jc.PlannedStartDate,
            PlannedEndDate   = jc.PlannedEndDate,
            MachineName      = jc.Machine?.Name,
            Shift            = jc.Shift,
            TargetQty        = jc.TargetQty,
            Status           = jc.Status.ToString(),
            Remarks          = jc.Remarks,
            WorkOrders = jc.WorkOrders.Select(w => new WorkOrderDto
            {
                Id             = w.Id,
                WONumber       = w.WONumber,
                JobCardId      = w.JobCardId,
                MachineName    = w.Machine?.Name,
                Shift          = w.Shift,
                ActualStartDate = w.ActualStartDate,
                ActualEndDate  = w.ActualEndDate,
                Status         = w.Status.ToString(),
                TotalProduced  = w.ProductionEntries.Sum(p => p.ProducedQty),
                TotalWastage   = w.ProductionEntries.Sum(p => p.WastageQty),
                Entries = w.ProductionEntries.Select(p => new ProductionEntryDto
                {
                    Id                = p.Id,
                    EntryDate         = p.EntryDate,
                    ProducedQty       = p.ProducedQty,
                    WastageQty        = p.WastageQty,
                    WastageReason     = p.WastageReason,
                    MachineDowntimeMin = p.MachineDowntimeMin,
                    DowntimeReason    = p.DowntimeReason
                }).ToList()
            }).ToList(),
            BOMs = jc.BOMs.Select(b => new BOMDto
            {
                Id        = b.Id,
                BOMNo     = b.BOMNo,
                JobCardId = b.JobCardId,
                Status    = b.Status,
                Remarks   = b.Remarks,
                Items = b.Items.Select(i => new BOMItemDto
                {
                    Id          = i.Id,
                    ItemId      = i.ItemId,
                    ItemCode    = i.Item?.ItemCode ?? "",
                    ItemName    = i.Item?.Name ?? "",
                    RequiredQty = i.RequiredQty,
                    IssuedQty   = i.IssuedQty
                }).ToList()
            }).ToList()
        };
    }

    public async Task<(bool Success, string Message, int Id)> CreateJobCardAsync(
        CreateJobCardDto dto, string createdBy)
    {
        var jcNo = await _numberService.GenerateAsync("JC");

        var jc = new JobCard
        {
            JobCardNo        = jcNo,
            SOId             = dto.SOId,
            CustomerJobId    = dto.CustomerJobId,
            PlannedStartDate = dto.PlannedStartDate,
            PlannedEndDate   = dto.PlannedEndDate,
            MachineId        = dto.MachineId,
            AssignedOperatorId = dto.AssignedOperatorId,
            Shift            = dto.Shift,
            TargetQty        = dto.TargetQty,
            UoMId            = dto.UoMId,
            Status           = JobCardStatus.Created,
            Remarks          = dto.Remarks,
            CreatedBy        = createdBy,
            CreatedAt        = DateTime.UtcNow
        };

        _context.JobCards.Add(jc);

        // Update SO status to InProduction
        if (dto.SOId.HasValue)
        {
            var so = await _context.SalesOrders.FindAsync(dto.SOId.Value);
            if (so != null && so.Status == SalesOrderStatus.Open)
            {
                so.Status    = SalesOrderStatus.InProduction;
                so.UpdatedAt = DateTime.UtcNow;
                so.UpdatedBy = createdBy;
            }
        }

        await _context.SaveChangesAsync();
        return (true, $"Job Card {jcNo} created.", jc.Id);
    }

    public async Task<(bool Success, string Message)> UpdateJobCardStatusAsync(
        int id, string status, string updatedBy)
    {
        var jc = await _context.JobCards.FindAsync(id);
        if (jc == null) return (false, "Job Card not found.");

        if (!Enum.TryParse<JobCardStatus>(status, out var parsed))
            return (false, "Invalid status.");

        jc.Status    = parsed;
        jc.UpdatedBy = updatedBy;
        jc.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, $"Status updated to {status}.");
    }

    // ── BOM ───────────────────────────────────────────────────────────────
    public async Task<(bool Success, string Message, int Id)> CreateBOMAsync(
        CreateBOMDto dto, string createdBy)
    {
        if (!dto.Items.Any())
            return (false, "Add at least one item.", 0);

        var bomNo = await _numberService.GenerateAsync("WO");

        var bom = new BOM
        {
            BOMNo         = $"BOM-{bomNo}",
            JobCardId     = dto.JobCardId,
            RequestedById = createdBy,
            RequestedAt   = DateTime.UtcNow,
            Status        = "Pending",
            Remarks       = dto.Remarks,
            CreatedBy     = createdBy,
            CreatedAt     = DateTime.UtcNow,
            Items = dto.Items.Select(i => new BOMItem
            {
                ItemId      = i.ItemId,
                RequiredQty = i.RequiredQty,
                IssuedQty   = 0,
                UoMId       = i.UoMId,
                CreatedBy   = createdBy,
                CreatedAt   = DateTime.UtcNow
            }).ToList()
        };

        _context.BOMs.Add(bom);
        await _context.SaveChangesAsync();
        return (true, $"BOM {bom.BOMNo} created.", bom.Id);
    }

    // ── WORK ORDERS ───────────────────────────────────────────────────────
    public async Task<List<WorkOrderDto>> GetWorkOrdersAsync(string? status = null)
    {
        var query = _context.WorkOrders
            .Include(w => w.Machine)
            .Include(w => w.ProductionEntries)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<WorkOrderStatus>(status, out var parsed))
            query = query.Where(w => w.Status == parsed);

        return await query.OrderByDescending(w => w.CreatedAt)
            .Select(w => new WorkOrderDto
            {
                Id             = w.Id,
                WONumber       = w.WONumber,
                JobCardId      = w.JobCardId,
                MachineName    = w.Machine != null ? w.Machine.Name : null,
                Shift          = w.Shift,
                ActualStartDate = w.ActualStartDate,
                ActualEndDate  = w.ActualEndDate,
                Status         = w.Status.ToString(),
                TotalProduced  = w.ProductionEntries.Sum(p => p.ProducedQty),
                TotalWastage   = w.ProductionEntries.Sum(p => p.WastageQty)
            }).ToListAsync();
    }

    public async Task<WorkOrderDto?> GetWorkOrderByIdAsync(int id)
    {
        var wo = await _context.WorkOrders
            .Include(w => w.Machine)
            .Include(w => w.JobCard)
            .Include(w => w.ProductionEntries)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (wo == null) return null;

        var so = wo.JobCard?.SOId.HasValue == true
            ? await _context.SalesOrders
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == wo.JobCard!.SOId!.Value)
            : null;

        return new WorkOrderDto
        {
            Id              = wo.Id,
            WONumber        = wo.WONumber,
            JobCardId       = wo.JobCardId,
            JobCardNo       = wo.JobCard?.JobCardNo,
            SONumber        = so?.SONumber,
            SODate          = so?.SODate,
            SORequiredBy    = so?.RequiredByDate,
            SOCustomerName  = so?.Customer?.Name,
            SOStatus        = so?.Status.ToString(),
            MachineName     = wo.Machine?.Name,
            Shift           = wo.Shift,
            ActualStartDate = wo.ActualStartDate,
            ActualEndDate   = wo.ActualEndDate,
            Status          = wo.Status.ToString(),
            TotalProduced   = wo.ProductionEntries.Sum(p => p.ProducedQty),
            TotalWastage    = wo.ProductionEntries.Sum(p => p.WastageQty),
            Entries = wo.ProductionEntries.Select(p => new ProductionEntryDto
            {
                Id                 = p.Id,
                EntryDate          = p.EntryDate,
                ProducedQty        = p.ProducedQty,
                WastageQty         = p.WastageQty,
                WastageReason      = p.WastageReason,
                MachineDowntimeMin = p.MachineDowntimeMin,
                DowntimeReason     = p.DowntimeReason
            }).ToList()
        };
    }

    public async Task<(bool Success, string Message, int Id)> CreateWorkOrderAsync(
        CreateWorkOrderDto dto, string createdBy)
    {
        var jc = await _context.JobCards.FindAsync(dto.JobCardId);
        if (jc == null) return (false, "Job Card not found.", 0);

        var woNo = await _numberService.GenerateAsync("WO");

        var wo = new WorkOrder
        {
            WONumber    = woNo,
            JobCardId   = dto.JobCardId,
            MachineId   = dto.MachineId,
            OperatorId  = dto.OperatorId,
            Shift       = dto.Shift,
            Status      = WorkOrderStatus.Open,
            CreatedBy   = createdBy,
            CreatedAt   = DateTime.UtcNow
        };

        _context.WorkOrders.Add(wo);

        jc.Status    = JobCardStatus.InProgress;
        jc.UpdatedAt = DateTime.UtcNow;
        jc.UpdatedBy = createdBy;

        await _context.SaveChangesAsync();
        return (true, $"Work Order {woNo} created.", wo.Id);
    }

    public async Task<(bool Success, string Message)> UpdateWorkOrderStatusAsync(
        int id, string status, string updatedBy)
    {
        var wo = await _context.WorkOrders.FindAsync(id);
        if (wo == null) return (false, "Work Order not found.");

        if (!Enum.TryParse<WorkOrderStatus>(status, out var parsed))
            return (false, "Invalid status.");

        wo.Status    = parsed;
        wo.UpdatedBy = updatedBy;
        wo.UpdatedAt = DateTime.UtcNow;

        if (parsed == WorkOrderStatus.Running && wo.ActualStartDate == null)
            wo.ActualStartDate = DateTime.UtcNow;

        if (parsed == WorkOrderStatus.Completed)
            wo.ActualEndDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, $"Work Order status updated to {status}.");
    }

    // ── PRODUCTION ENTRIES ────────────────────────────────────────────────
    public async Task<(bool Success, string Message)> AddProductionEntryAsync(
        CreateProductionEntryDto dto, string createdBy)
    {
        var wo = await _context.WorkOrders.FindAsync(dto.WorkOrderId);
        if (wo == null) return (false, "Work Order not found.");

        if (wo.Status == WorkOrderStatus.Completed)
            return (false, "Cannot add entry to a completed Work Order.");

        _context.ProductionEntries.Add(new ProductionEntry
        {
            WorkOrderId        = dto.WorkOrderId,
            EntryDate          = DateTime.UtcNow,
            ProducedQty        = dto.ProducedQty,
            WastageQty         = dto.WastageQty,
            WastageReason      = dto.WastageReason,
            MachineDowntimeMin = dto.MachineDowntimeMin,
            DowntimeReason     = dto.DowntimeReason,
            OperatorId         = dto.OperatorId ?? createdBy,
            CreatedBy          = createdBy,
            CreatedAt          = DateTime.UtcNow
        });

        if (wo.Status == WorkOrderStatus.Open)
        {
            wo.Status        = WorkOrderStatus.Running;
            wo.ActualStartDate = DateTime.UtcNow;
        }

        wo.UpdatedAt = DateTime.UtcNow;
        wo.UpdatedBy = createdBy;

        await _context.SaveChangesAsync();
        return (true, "Production entry saved.");
    }

    public async Task<(bool Success, string Message)> CompleteWorkOrderAsync(
        int workOrderId, int fgWarehouseId, string createdBy)
    {
        var wo = await _context.WorkOrders
            .Include(w => w.ProductionEntries)
            .Include(w => w.JobCard)
            .FirstOrDefaultAsync(w => w.Id == workOrderId);

        if (wo == null) return (false, "Work Order not found.");
        if (wo.Status == WorkOrderStatus.Completed)
            return (false, "Already completed.");

        var totalProduced = wo.ProductionEntries.Sum(p => p.ProducedQty);
        if (totalProduced <= 0)
            return (false, "No production entries found. Add at least one entry first.");

        wo.Status      = WorkOrderStatus.Completed;
        wo.ActualEndDate = DateTime.UtcNow;
        wo.UpdatedAt   = DateTime.UtcNow;
        wo.UpdatedBy   = createdBy;

        // Move produced qty to FG warehouse
        if (wo.JobCard.SOId.HasValue)
        {
            // Use SO item's item as FG — simplified: use jobcard's UoM item
            // Full implementation would use BOM output item
            // For now we receive stock against WO reference
        }

        // Mark JobCard as Completed
        wo.JobCard.Status    = JobCardStatus.Completed;
        wo.JobCard.UpdatedAt = DateTime.UtcNow;
        wo.JobCard.UpdatedBy = createdBy;

        await _context.SaveChangesAsync();
        return (true, $"Work Order completed. Total produced: {totalProduced}");
    }
}