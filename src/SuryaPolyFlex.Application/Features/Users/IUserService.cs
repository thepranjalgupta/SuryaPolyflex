namespace SuryaPolyFlex.Application.Features.Users;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync(string? search = null);
    Task<UserDto?> GetByIdAsync(string id);
    Task<(bool Success, string Message)> CreateAsync(CreateUserDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateAsync(EditUserDto dto);
    Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto dto);
    Task<(bool Success, string Message)> ToggleActiveAsync(string userId);
    Task<List<string>> GetAllRolesAsync();
}