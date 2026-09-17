using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.Users;

namespace SmartFlow.Infrastructure.Identity;

public sealed class UserAdministrationService(
    UserManager<ApplicationUser> userManager)
    : IUserAdministrationService
{
    public async Task<IReadOnlyCollection<UserDto>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken)
    {
        IQueryable<ApplicationUser> query = userManager.Users;

        var normalizedSearch = search?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(user =>
                EF.Functions.ILike(
                    user.Email!,
                    $"%{normalizedSearch}%") ||
                EF.Functions.ILike(
                    user.FullName,
                    $"%{normalizedSearch}%"));
        }

        var users = await query
            .OrderBy(user => user.FullName)
            .ThenBy(user => user.Email)
            .ToListAsync(cancellationToken);

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            result.Add(await MapAsync(user));
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        return user is null ? null : await MapAsync(user);
    }

    public async Task<UserDto> CreateAsync(
        string fullName,
        string email,
        string password,
        string role,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var existingUser = await userManager.FindByEmailAsync(
            normalizedEmail);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "An account already exists with this email address.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = normalizedEmail,
            UserName = normalizedEmail,
            EmailConfirmed = true,
            IsActive = true
        };

        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    " ",
                    createResult.Errors.Select(error => error.Description)));
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, role);

        if (!addRoleResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    " ",
                    addRoleResult.Errors.Select(error => error.Description)));
        }

        return await MapAsync(user);
    }

    public async Task<UserDto?> ChangeRoleAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return null;
        }

        var currentRoles = await userManager.GetRolesAsync(user);

        if (currentRoles.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(
                user,
                currentRoles);

            if (!removeResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Unable to remove the current user role.");
            }
        }

        var addResult = await userManager.AddToRoleAsync(user, role);

        if (!addResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    " ",
                    addResult.Errors.Select(error => error.Description)));
        }

        return await MapAsync(user);
    }

    public async Task<UserDto?> ChangeStatusAsync(
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return null;
        }

        user.IsActive = isActive;

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    " ",
                    updateResult.Errors.Select(error => error.Description)));
        }

        return await MapAsync(user);
    }

    private async Task<UserDto> MapAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        return new UserDto(
            user.Id,
            user.FullName,
            user.Email!,
            user.IsActive,
            roles.ToArray(),
            user.LockoutEnd);
    }
}