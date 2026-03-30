using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Features.Leads;
using SuryaPolyFlex.Domain.Entities.Sales;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class LeadService : ILeadService
{
    private readonly AppDbContext _context;
    public LeadService(AppDbContext context) => _context = context;

    public async Task<List<LeadDto>> GetAllAsync(string? status = null)
    {
        var query = _context.Leads
            .Include(l => l.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<LeadStatus>(status, out var parsed))
            query = query.Where(l => l.Status == parsed);

        return await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new LeadDto
            {
                Id            = l.Id,
                Title         = l.Title,
                CustomerId    = l.CustomerId,
                CustomerName  = l.Customer.Name,
                ContactPerson = l.ContactPerson,
                Phone         = l.Phone,
                Email         = l.Email,
                Source        = l.Source,
                Status        = l.Status.ToString(),
                FollowUpDate  = l.FollowUpDate,
                Remarks       = l.Remarks,
                CreatedAt     = l.CreatedAt
            }).ToListAsync();
    }

    public async Task<LeadDto?> GetByIdAsync(int id)
    {
        return await _context.Leads
            .Include(l => l.Customer)
            .Where(l => l.Id == id)
            .Select(l => new LeadDto
            {
                Id            = l.Id,
                Title         = l.Title,
                CustomerId    = l.CustomerId,
                CustomerName  = l.Customer.Name,
                ContactPerson = l.ContactPerson,
                Phone         = l.Phone,
                Email         = l.Email,
                Source        = l.Source,
                Status        = l.Status.ToString(),
                FollowUpDate  = l.FollowUpDate,
                Remarks       = l.Remarks,
                CreatedAt     = l.CreatedAt
            }).FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateLeadDto dto, string createdBy)
    {
        _context.Leads.Add(new Lead
        {
            Title         = dto.Title.Trim(),
            CustomerId    = dto.CustomerId,
            ContactPerson = dto.ContactPerson?.Trim(),
            Phone         = dto.Phone?.Trim(),
            Email         = dto.Email?.Trim(),
            Source        = dto.Source?.Trim(),
            Status        = LeadStatus.New,
            FollowUpDate  = dto.FollowUpDate,
            Remarks       = dto.Remarks,
            CreatedBy     = createdBy,
            CreatedAt     = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Lead created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        EditLeadDto dto, string updatedBy)
    {
        var lead = await _context.Leads.FindAsync(dto.Id);
        if (lead == null) return (false, "Lead not found.");

        lead.Title         = dto.Title.Trim();
        lead.CustomerId    = dto.CustomerId;
        lead.ContactPerson = dto.ContactPerson?.Trim();
        lead.Phone         = dto.Phone?.Trim();
        lead.Email         = dto.Email?.Trim();
        lead.Source        = dto.Source?.Trim();
        lead.FollowUpDate  = dto.FollowUpDate;
        lead.Remarks       = dto.Remarks;
        lead.UpdatedBy     = updatedBy;
        lead.UpdatedAt     = DateTime.UtcNow;

        if (Enum.TryParse<LeadStatus>(dto.Status, out var status))
            lead.Status = status;

        await _context.SaveChangesAsync();
        return (true, "Lead updated successfully.");
    }

    public async Task<(bool Success, string Message)> ConvertToCustomerAsync(
        int id, string updatedBy)
    {
        var lead = await _context.Leads.FindAsync(id);
        if (lead == null) return (false, "Lead not found.");

        lead.Status      = LeadStatus.Converted;
        lead.ConvertedAt = DateTime.UtcNow;
        lead.UpdatedBy   = updatedBy;
        lead.UpdatedAt   = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, "Lead marked as converted.");
    }
}