using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(ApplicationDbContext context, ITokenService tokenService, IMapper mapper)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<LoginResponseDto?> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        if (!user.IsActive)
        {
            return null;
        }

        var accessToken = _tokenService.GenerateAccessToken(user);

        var userDto = _mapper.Map<UserDto>(user);
        return new LoginResponseDto
        {
            User = userDto,
            Token = accessToken
        };
    }

    public async Task<User?> RegisterAsync(string username, string email, string password, string fullName, string role = "User")
    {
        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
        {
            return null;
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = fullName,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
