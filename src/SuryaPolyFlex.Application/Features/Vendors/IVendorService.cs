namespace SuryaPolyFlex.Application.Features.Vendors;

public interface IVendorService
{
    Task<List<VendorDto>> GetAllAsync(string? search = null);
    Task<VendorDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> CreateAsync(CreateVendorDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateAsync(EditVendorDto dto, string updatedBy);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}