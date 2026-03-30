using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Leads;

public class LeadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Source { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? FollowUpDate { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLeadDto
{
    [Required] public string Title { get; set; } = default!;
    [Required] public int CustomerId { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Source { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? Remarks { get; set; }
}

public class EditLeadDto : CreateLeadDto
{
    public int Id { get; set; }
    public string Status { get; set; } = "New";
}