using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using EventPlanner.Application.Auth.Abstractions;
using EventPlanner.Application.Auth.Models;
using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Domain.Entities;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EventPlanner.Infrastructure.Auth;

public sealed class JwtTokenService(IOptions<JwtOptions> options, IClock clock) : IAuthTokenService
{
    private readonly IClock _clock = clock;
    private readonly JwtOptions _options = options.Value;

    public AuthToken CreateToken(User user)
    {
        if (string.IsNullOrWhiteSpace(_options.Secret))
        {
            throw new InvalidOperationException("JWT secret is not configured.");
        }

        var now = _clock.UtcNow;
        var expiresAt = now.AddMinutes(_options.AccessTokenMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials
        );

        return new AuthToken(
            new JwtSecurityTokenHandler().WriteToken(token),
            _options.AccessTokenMinutes * 60
        );
    }
}
