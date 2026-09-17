namespace SmartFlow.Application.Users;

public interface IUserAdministrationService
{
    Task<IReadOnlyCollection<UserDto>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken);

    Task<UserDto?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<UserDto> CreateAsync(
        string fullName,
        string email,
        string password,
        string role,
        CancellationToken cancellationToken);

    Task<UserDto?> ChangeRoleAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken);

    Task<UserDto?> ChangeStatusAsync(
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken);
}