using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartFlow.Application.Authentication;
using SmartFlow.Application.Common.Models;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Infrastructure.Identity;

public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtOptions> jwtOptions)
    : IAuthenticationService
{
    public async Task<AuthResult> RegisterAsync(
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var existingUser = await userManager.FindByEmailAsync(normalizedEmail);

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
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                " ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }

        await userManager.AddToRoleAsync(user, Roles.Collaborateur);

        return await CreateAuthResultAsync(user);
    }

    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await userManager.FindByEmailAsync(normalizedEmail);

        if (user is null ||
            !await userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        return await CreateAuthResultAsync(user);
    }

    private async Task<AuthResult> CreateAuthResultAsync(
        ApplicationUser user)
    {
        var options = jwtOptions.Value;
        var roles = await userManager.GetRolesAsync(user);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(
            options.ExpirationMinutes);

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

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new AuthResult(
            user.Id,
            user.FullName,
            user.Email!,
            roles.ToArray(),
            accessToken,
            expiresAtUtc);
    }
}