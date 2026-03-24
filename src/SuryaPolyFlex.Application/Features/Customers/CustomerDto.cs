using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Customers;

public class CustomerDto
{
    public int Id { get; set; }
    public string CustomerCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? GSTIN { get; set; }
    public decimal CreditLimit { get; set; }
    public int PaymentTermDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCustomerDto
{
    [Required] public string CustomerCode { get; set; } = default!;
    [Required] public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    [EmailAddress] public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? BillingAddress { get; set; }
    public string? ShippingAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? GSTIN { get; set; }
    public string? PAN { get; set; }
    public decimal CreditLimit { get; set; } = 0;
    public int PaymentTermDays { get; set; } = 30;
}

public class EditCustomerDto : CreateCustomerDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}