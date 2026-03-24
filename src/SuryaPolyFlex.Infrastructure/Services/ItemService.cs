using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Features.Items;
using SuryaPolyFlex.Domain.Entities.Inventory;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class ItemService : IItemService
{
    private readonly AppDbContext _context;
    public ItemService(AppDbContext context) => _context = context;

    public async Task<List<ItemDto>> GetAllAsync(string? search = null, string? itemType = null)
    {
        var query = _context.Items
            .Include(i => i.Category)
            .Include(i => i.UoM)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i =>
                i.Name.Contains(search) || i.ItemCode.Contains(search));

        if (!string.IsNullOrWhiteSpace(itemType))
            query = query.Where(i => i.ItemType == itemType);

        return await query.OrderBy(i => i.Name).Select(i => new ItemDto
        {
            Id           = i.Id,
            ItemCode     = i.ItemCode,
            Name         = i.Name,
            Description  = i.Description,
            CategoryId   = i.CategoryId,
            CategoryName = i.Category.Name,
            UoMId        = i.UoMId,
            UoMCode      = i.UoM.Code,
            ItemType     = i.ItemType,
            MinStockLevel = i.MinStockLevel,
            ReorderQty   = i.ReorderQty,
            StandardCost = i.StandardCost,
            IsActive     = i.IsActive
        }).ToListAsync();
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.UoM)
            .Where(i => i.Id == id)
            .Select(i => new ItemDto
            {
                Id           = i.Id,
                ItemCode     = i.ItemCode,
                Name         = i.Name,
                Description  = i.Description,
                CategoryId   = i.CategoryId,
                CategoryName = i.Category.Name,
                UoMId        = i.UoMId,
                UoMCode      = i.UoM.Code,
                ItemType     = i.ItemType,
                MinStockLevel = i.MinStockLevel,
                ReorderQty   = i.ReorderQty,
                StandardCost = i.StandardCost,
                IsActive     = i.IsActive
            }).FirstOrDefaultAsync();
    }

    public async Task<List<ItemSelectDto>> GetSelectListAsync()
    {
        return await _context.Items
            .Include(i => i.UoM)
            .Where(i => i.IsActive)
            .OrderBy(i => i.Name)
            .Select(i => new ItemSelectDto
            {
                Id       = i.Id,
                ItemCode = i.ItemCode,
                Name     = i.Name,
                UoMCode  = i.UoM.Code
            }).ToListAsync();
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateItemDto dto, string createdBy)
    {
        if (await _context.Items.AnyAsync(i => i.ItemCode == dto.ItemCode.ToUpper()))
            return (false, $"Item code '{dto.ItemCode}' already exists.");

        _context.Items.Add(new Item
        {
            ItemCode      = dto.ItemCode.ToUpper().Trim(),
            Name          = dto.Name.Trim(),
            Description   = dto.Description?.Trim(),
            CategoryId    = dto.CategoryId,
            UoMId         = dto.UoMId,
            ItemType      = dto.ItemType,
            MinStockLevel = dto.MinStockLevel,
            ReorderQty    = dto.ReorderQty,
            StandardCost  = dto.StandardCost,
            IsActive      = true,
            CreatedBy     = createdBy,
            CreatedAt     = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Item created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        EditItemDto dto, string updatedBy)
    {
        var item = await _context.Items.FindAsync(dto.Id);
        if (item == null) return (false, "Item not found.");

        if (await _context.Items.AnyAsync(i =>
            i.ItemCode == dto.ItemCode.ToUpper() && i.Id != dto.Id))
            return (false, $"Item code '{dto.ItemCode}' already exists.");

        item.ItemCode      = dto.ItemCode.ToUpper().Trim();
        item.Name          = dto.Name.Trim();
        item.Description   = dto.Description?.Trim();
        item.CategoryId    = dto.CategoryId;
        item.UoMId         = dto.UoMId;
        item.ItemType      = dto.ItemType;
        item.MinStockLevel = dto.MinStockLevel;
        item.ReorderQty    = dto.ReorderQty;
        item.StandardCost  = dto.StandardCost;
        item.IsActive      = dto.IsActive;
        item.UpdatedBy     = updatedBy;
        item.UpdatedAt     = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, "Item updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return (false, "Item not found.");

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Item deleted.");
    }

    public async Task<List<(int Id, string Code, string Name)>> GetCategoriesAsync()
    {
        return await _context.ItemCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new ValueTuple<int, string, string>(c.Id, c.Code, c.Name))
            .ToListAsync();
    }

    public async Task<List<(int Id, string Code, string Name)>> GetUoMsAsync()
    {
        return await _context.UnitOfMeasures
            .Where(u => u.IsActive)
            .OrderBy(u => u.Name)
            .Select(u => new ValueTuple<int, string, string>(u.Id, u.Code, u.Name))
            .ToListAsync();
    }
}