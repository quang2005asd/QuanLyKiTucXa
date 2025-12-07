using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

[Authorize(Policy = "AdminOnly")]
public class UsersController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UsersController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<UserDto>>> GetUsers(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<UserDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Users.CountAsync();
        var users = await _context.Users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userDtos = _mapper.Map<List<UserDto>>(users);
        return OkPaginatedResponse(userDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFoundResponse<UserDto>("User not found");

        var userDto = _mapper.Map<UserDto>(user);
        return OkResponse(userDto);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> PostUser(CreateUserDto createUserDto)
    {
        var validationError = ValidateModelState<UserDto>();
        if (validationError != null)
            return validationError;

        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username))
            return BadRequestResponse<UserDto>("Username already exists");

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
            return BadRequestResponse<UserDto>("Email already exists");

        var user = _mapper.Map<User>(createUserDto);
        // Hash password - using simple bcrypt in production
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = _mapper.Map<UserDto>(user);
        return CreatedResponse("GetUser", new { id = user.Id }, userDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> PutUser(int id, UpdateUserDto updateUserDto)
    {
        var validationError = ValidateModelState<UserDto>();
        if (validationError != null)
            return validationError;

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFoundResponse<UserDto>("User not found");

        // Check if email is being updated and already exists
        if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email))
                return BadRequestResponse<UserDto>("Email already exists");
        }

        _mapper.Map(updateUserDto, user);
        user.UpdatedAt = DateTime.UtcNow;

        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
                return NotFoundResponse<UserDto>("User not found");
            throw;
        }

        var userDto = _mapper.Map<UserDto>(user);
        return OkResponse(userDto, "User updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFoundResponse<object>("User not found");

        // Soft delete
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return OkResponse<object>(new { }, "User deleted successfully");
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
