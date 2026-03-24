namespace SuryaPolyFlex.Application.Features.Items;

public interface IItemService
{
    Task<List<ItemDto>> GetAllAsync(string? search = null, string? itemType = null);
    Task<ItemDto?> GetByIdAsync(int id);
    Task<List<ItemSelectDto>> GetSelectListAsync();
    Task<(bool Success, string Message)> CreateAsync(CreateItemDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateAsync(EditItemDto dto, string updatedBy);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<List<(int Id, string Code, string Name)>> GetCategoriesAsync();
    Task<List<(int Id, string Code, string Name)>> GetUoMsAsync();
}