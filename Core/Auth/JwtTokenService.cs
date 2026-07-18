using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Core.Auth;

public sealed class JwtTokenService(IConfiguration configuration)
{
    public string Create(User user)
    {
        var detail = user.Detail;
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("firstName", detail?.FirstName ?? string.Empty),
            new Claim("lastName", detail?.LastName ?? string.Empty),
            new Claim("branchId", user.BranchId.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!));
        var token = new JwtSecurityToken(configuration["JwtSettings:Issuer"], configuration["JwtSettings:Audience"], claims,
            expires: DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("JwtSettings:ExpiryMinutes")),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
}
