using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Features.Customers;
using SuryaPolyFlex.Domain.Entities.Sales;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context) => _context = context;

    public async Task<List<CustomerDto>> GetAllAsync(string? search = null)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.Name.Contains(search) ||
                c.CustomerCode.Contains(search) ||
                (c.Mobile != null && c.Mobile.Contains(search)) ||
                (c.GSTIN  != null && c.GSTIN.Contains(search)));

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CustomerDto
            {
                Id              = c.Id,
                CustomerCode    = c.CustomerCode,
                Name            = c.Name,
                ContactPerson   = c.ContactPerson,
                Email           = c.Email,
                Mobile          = c.Mobile,
                City            = c.City,
                State           = c.State,
                GSTIN           = c.GSTIN,
                CreditLimit     = c.CreditLimit,
                PaymentTermDays = c.PaymentTermDays,
                IsActive        = c.IsActive,
                CreatedAt       = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .Where(c => c.Id == id)
            .Select(c => new CustomerDto
            {
                Id              = c.Id,
                CustomerCode    = c.CustomerCode,
                Name            = c.Name,
                ContactPerson   = c.ContactPerson,
                Email           = c.Email,
                Mobile          = c.Mobile,
                City            = c.City,
                State           = c.State,
                GSTIN           = c.GSTIN,
                CreditLimit     = c.CreditLimit,
                PaymentTermDays = c.PaymentTermDays,
                IsActive        = c.IsActive,
                CreatedAt       = c.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateCustomerDto dto, string createdBy)
    {
        if (await _context.Customers.AnyAsync(c =>
            c.CustomerCode == dto.CustomerCode.ToUpper()))
            return (false, $"Customer code '{dto.CustomerCode}' already exists.");

        _context.Customers.Add(new Customer
        {
            CustomerCode    = dto.CustomerCode.ToUpper().Trim(),
            Name            = dto.Name.Trim(),
            ContactPerson   = dto.ContactPerson?.Trim(),
            Email           = dto.Email?.Trim(),
            Phone           = dto.Phone?.Trim(),
            Mobile          = dto.Mobile?.Trim(),
            BillingAddress  = dto.BillingAddress?.Trim(),
            ShippingAddress = dto.ShippingAddress?.Trim(),
            City            = dto.City?.Trim(),
            State           = dto.State?.Trim(),
            PinCode         = dto.PinCode?.Trim(),
            GSTIN           = dto.GSTIN?.Trim().ToUpper(),
            PAN             = dto.PAN?.Trim().ToUpper(),
            CreditLimit     = dto.CreditLimit,
            PaymentTermDays = dto.PaymentTermDays,
            IsActive        = true,
            CreatedBy       = createdBy,
            CreatedAt       = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Customer created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        EditCustomerDto dto, string updatedBy)
    {
        var customer = await _context.Customers.FindAsync(dto.Id);
        if (customer == null) return (false, "Customer not found.");

        if (await _context.Customers.AnyAsync(c =>
            c.CustomerCode == dto.CustomerCode.ToUpper() && c.Id != dto.Id))
            return (false, $"Customer code '{dto.CustomerCode}' already exists.");

        customer.CustomerCode    = dto.CustomerCode.ToUpper().Trim();
        customer.Name            = dto.Name.Trim();
        customer.ContactPerson   = dto.ContactPerson?.Trim();
        customer.Email           = dto.Email?.Trim();
        customer.Phone           = dto.Phone?.Trim();
        customer.Mobile          = dto.Mobile?.Trim();
        customer.BillingAddress  = dto.BillingAddress?.Trim();
        customer.ShippingAddress = dto.ShippingAddress?.Trim();
        customer.City            = dto.City?.Trim();
        customer.State           = dto.State?.Trim();
        customer.PinCode         = dto.PinCode?.Trim();
        customer.GSTIN           = dto.GSTIN?.Trim().ToUpper();
        customer.PAN             = dto.PAN?.Trim().ToUpper();
        customer.CreditLimit     = dto.CreditLimit;
        customer.PaymentTermDays = dto.PaymentTermDays;
        customer.IsActive        = dto.IsActive;
        customer.UpdatedBy       = updatedBy;
        customer.UpdatedAt       = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, "Customer updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return (false, "Customer not found.");

        customer.IsDeleted = true;
        customer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Customer deleted.");
    }
}