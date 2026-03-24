using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Features.Vendors;
using SuryaPolyFlex.Domain.Entities.Procurement;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class VendorService : IVendorService
{
    private readonly AppDbContext _context;

    public VendorService(AppDbContext context) => _context = context;

    public async Task<List<VendorDto>> GetAllAsync(string? search = null)
    {
        var query = _context.Vendors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(v =>
                v.Name.Contains(search) ||
                v.VendorCode.Contains(search) ||
                (v.Mobile != null && v.Mobile.Contains(search)) ||
                (v.GSTIN  != null && v.GSTIN.Contains(search)));

        return await query
            .OrderBy(v => v.Name)
            .Select(v => new VendorDto
            {
                Id             = v.Id,
                VendorCode     = v.VendorCode,
                Name           = v.Name,
                ContactPerson  = v.ContactPerson,
                Email          = v.Email,
                Phone          = v.Phone,
                Mobile         = v.Mobile,
                City           = v.City,
                State          = v.State,
                GSTIN          = v.GSTIN,
                PaymentTermDays = v.PaymentTermDays,
                IsActive       = v.IsActive,
                CreatedAt      = v.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<VendorDto?> GetByIdAsync(int id)
    {
        return await _context.Vendors
            .Where(v => v.Id == id)
            .Select(v => new VendorDto
            {
                Id             = v.Id,
                VendorCode     = v.VendorCode,
                Name           = v.Name,
                ContactPerson  = v.ContactPerson,
                Email          = v.Email,
                Phone          = v.Phone,
                Mobile         = v.Mobile,
                City           = v.City,
                State          = v.State,
                GSTIN          = v.GSTIN,
                PaymentTermDays = v.PaymentTermDays,
                IsActive       = v.IsActive,
                CreatedAt      = v.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateVendorDto dto, string createdBy)
    {
        if (await _context.Vendors.AnyAsync(v => v.VendorCode == dto.VendorCode.ToUpper()))
            return (false, $"Vendor code '{dto.VendorCode}' already exists.");

        _context.Vendors.Add(new Vendor
        {
            VendorCode      = dto.VendorCode.ToUpper().Trim(),
            Name            = dto.Name.Trim(),
            ContactPerson   = dto.ContactPerson?.Trim(),
            Email           = dto.Email?.Trim(),
            Phone           = dto.Phone?.Trim(),
            Mobile          = dto.Mobile?.Trim(),
            Address         = dto.Address?.Trim(),
            City            = dto.City?.Trim(),
            State           = dto.State?.Trim(),
            PinCode         = dto.PinCode?.Trim(),
            GSTIN           = dto.GSTIN?.Trim().ToUpper(),
            PAN             = dto.PAN?.Trim().ToUpper(),
            BankName        = dto.BankName?.Trim(),
            BankAccount     = dto.BankAccount?.Trim(),
            BankIFSC        = dto.BankIFSC?.Trim().ToUpper(),
            PaymentTermDays = dto.PaymentTermDays,
            IsActive        = true,
            CreatedBy       = createdBy,
            CreatedAt       = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Vendor created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        EditVendorDto dto, string updatedBy)
    {
        var vendor = await _context.Vendors.FindAsync(dto.Id);
        if (vendor == null) return (false, "Vendor not found.");

        if (await _context.Vendors.AnyAsync(v =>
            v.VendorCode == dto.VendorCode.ToUpper() && v.Id != dto.Id))
            return (false, $"Vendor code '{dto.VendorCode}' already exists.");

        vendor.VendorCode      = dto.VendorCode.ToUpper().Trim();
        vendor.Name            = dto.Name.Trim();
        vendor.ContactPerson   = dto.ContactPerson?.Trim();
        vendor.Email           = dto.Email?.Trim();
        vendor.Phone           = dto.Phone?.Trim();
        vendor.Mobile          = dto.Mobile?.Trim();
        vendor.Address         = dto.Address?.Trim();
        vendor.City            = dto.City?.Trim();
        vendor.State           = dto.State?.Trim();
        vendor.PinCode         = dto.PinCode?.Trim();
        vendor.GSTIN           = dto.GSTIN?.Trim().ToUpper();
        vendor.PAN             = dto.PAN?.Trim().ToUpper();
        vendor.BankName        = dto.BankName?.Trim();
        vendor.BankAccount     = dto.BankAccount?.Trim();
        vendor.BankIFSC        = dto.BankIFSC?.Trim().ToUpper();
        vendor.PaymentTermDays = dto.PaymentTermDays;
        vendor.IsActive        = dto.IsActive;
        vendor.UpdatedBy       = updatedBy;
        vendor.UpdatedAt       = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, "Vendor updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor == null) return (false, "Vendor not found.");

        vendor.IsDeleted = true;
        vendor.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Vendor deleted.");
    }
}