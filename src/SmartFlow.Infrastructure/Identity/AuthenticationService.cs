using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartFlow.Application.Authentication;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Models;
using SmartFlow.Application.Common.Security;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Identity;

public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IOptions<JwtOptions> jwtOptions)
    : IAuthenticationService
{
    private const int RefreshTokenLifetimeDays = 7;

    public async Task<AuthResult> RegisterAsync(
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (await userManager.FindByEmailAsync(normalizedEmail) is not null)
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

        await userManager.AddToRoleAsync(user, Roles.Collaborateur);

        return await CreateAuthResultAsync(user, cancellationToken);
    }

    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await userManager.FindByEmailAsync(normalizedEmail);

        if (user is null ||
            !await userManager.CheckPasswordAsync(user, password) ||
            !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        return await CreateAuthResultAsync(user, cancellationToken);
    }

    public async Task<AuthResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(refreshToken);

        var storedToken = await refreshTokenRepository.GetForUpdateAsync(
            tokenHash,
            cancellationToken);

        if (storedToken is null || !storedToken.IsUsable())
        {
            throw new UnauthorizedAccessException(
                "Refresh token is invalid or expired.");
        }

        var user = await userManager.FindByIdAsync(
            storedToken.UserId.ToString());

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User account is unavailable.");
        }

        storedToken.Revoke();

        return await CreateAuthResultAsync(user, cancellationToken);
    }

    public async Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(refreshToken);

        var storedToken = await refreshTokenRepository.GetForUpdateAsync(
            tokenHash,
            cancellationToken);

        if (storedToken is null)
        {
            return;
        }

        storedToken.Revoke();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthResult> CreateAuthResultAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var options = jwtOptions.Value;
        var roles = await userManager.GetRolesAsync(user);

        var jwtExpiresAtUtc = DateTime.UtcNow.AddMinutes(
            options.ExpirationMinutes);

        var refreshTokenValue = GenerateSecureToken();
        var refreshExpiresAtUtc = DateTime.UtcNow.AddDays(
            RefreshTokenLifetimeDays);

        var refreshToken = RefreshToken.Create(
            user.Id,
            HashToken(refreshTokenValue),
            refreshExpiresAtUtc);

        await refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName)
        };

        claims.AddRange(roles.Select(role =>
            new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.Key));

        var jwt = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: jwtExpiresAtUtc,
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256));

        return new AuthResult(
            user.Id,
            user.FullName,
            user.Email!,
            roles.ToArray(),
            new JwtSecurityTokenHandler().WriteToken(jwt),
            jwtExpiresAtUtc,
            refreshTokenValue,
            refreshExpiresAtUtc);
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}