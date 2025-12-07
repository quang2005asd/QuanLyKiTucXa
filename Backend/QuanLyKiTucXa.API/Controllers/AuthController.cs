using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Services;

namespace QuanLyKiTucXa.API.Controllers;

public class AuthController : BaseController
{
    private readonly AuthService _authService;
    private readonly IMapper _mapper;

    public AuthController(AuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        var validationError = ValidateModelState<LoginResponseDto>();
        if (validationError != null)
            return validationError;

        var result = await _authService.LoginAsync(loginDto.Username, loginDto.Password);

        if (result == null)
            return BadRequestResponse<LoginResponseDto>("Invalid credentials");

        return OkResponse(result, "Login successful");
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> Register([FromBody] CreateUserDto createUserDto)
    {
        var validationError = ValidateModelState<UserDto>();
        if (validationError != null)
            return validationError;

        var user = await _authService.RegisterAsync(
            createUserDto.Username,
            createUserDto.Email,
            createUserDto.Password,
            createUserDto.FullName,
            createUserDto.Role);

        if (user == null)
            return BadRequestResponse<UserDto>("User already exists or invalid data");

        var userDto = _mapper.Map<UserDto>(user);
        return OkResponse(userDto, "Registration successful");
    }
}
