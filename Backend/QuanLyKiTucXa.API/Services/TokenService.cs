using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var secret = _configuration["JwtSettings:Secret"] ?? "your-secret-key-here-change-this-in-production";
        var expireMinutes = int.Parse(_configuration["JwtSettings:ExpireMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"] ?? "QuanLyKiTucXa",
            audience: _configuration["JwtSettings:Audience"] ?? "QuanLyKiTucXaUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
