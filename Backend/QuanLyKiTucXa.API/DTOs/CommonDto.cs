using System.ComponentModel.DataAnnotations;

namespace QuanLyKiTucXa.API.DTOs;

/// <summary>
/// DTO for login response
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// DTO for login request
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}

/// <summary>
/// Standard API Response wrapper
/// </summary>
public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public static ApiResponseDto<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponseDto<T> ErrorResponse(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

/// <summary>
/// Pagination meta data
/// </summary>
public class PaginationMetaDto
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Paginated API Response
/// </summary>
public class PaginatedResponseDto<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<T> Data { get; set; } = new();
    public PaginationMetaDto Pagination { get; set; } = new();
    public Dictionary<string, string[]>? Errors { get; set; }

    public static PaginatedResponseDto<T> SuccessResponse(List<T> data, int totalCount, int pageNumber, int pageSize, string? message = null)
    {
        return new PaginatedResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = new PaginationMetaDto
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            }
        };
    }

    public static PaginatedResponseDto<T> ErrorResponse(string message, Dictionary<string, string[]>? errors = null)
    {
        return new PaginatedResponseDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
