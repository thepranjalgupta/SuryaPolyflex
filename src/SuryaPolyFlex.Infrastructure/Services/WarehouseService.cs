using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Features.Warehouses;
using SuryaPolyFlex.Domain.Entities.Inventory;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class WarehouseService : IWarehouseService
{
    private readonly AppDbContext _context;
    public WarehouseService(AppDbContext context) => _context = context;

    public async Task<List<WarehouseDto>> GetAllAsync()
    {
        return await _context.Warehouses
            .OrderBy(w => w.Name)
            .Select(w => new WarehouseDto
            {
                Id            = w.Id,
                Code          = w.Code,
                Name          = w.Name,
                Location      = w.Location,
                WarehouseType = w.WarehouseType,
                IsActive      = w.IsActive
            }).ToListAsync();
    }

    public async Task<WarehouseDto?> GetByIdAsync(int id)
    {
        return await _context.Warehouses
            .Where(w => w.Id == id)
            .Select(w => new WarehouseDto
            {
                Id            = w.Id,
                Code          = w.Code,
                Name          = w.Name,
                Location      = w.Location,
                WarehouseType = w.WarehouseType,
                IsActive      = w.IsActive
            }).FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateWarehouseDto dto, string createdBy)
    {
        if (await _context.Warehouses.AnyAsync(w => w.Code == dto.Code.ToUpper()))
            return (false, $"Warehouse code '{dto.Code}' already exists.");

        _context.Warehouses.Add(new Warehouse
        {
            Code          = dto.Code.ToUpper().Trim(),
            Name          = dto.Name.Trim(),
            Location      = dto.Location?.Trim(),
            WarehouseType = dto.WarehouseType,
            IsActive      = true,
            CreatedBy     = createdBy,
            CreatedAt     = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Warehouse created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        EditWarehouseDto dto, string updatedBy)
    {
        var warehouse = await _context.Warehouses.FindAsync(dto.Id);
        if (warehouse == null) return (false, "Warehouse not found.");

        warehouse.Code          = dto.Code.ToUpper().Trim();
        warehouse.Name          = dto.Name.Trim();
        warehouse.Location      = dto.Location?.Trim();
        warehouse.WarehouseType = dto.WarehouseType;
        warehouse.IsActive      = dto.IsActive;
        warehouse.UpdatedBy     = updatedBy;
        warehouse.UpdatedAt     = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, "Warehouse updated successfully.");
    }
}