using System.ComponentModel.DataAnnotations;

namespace QuanLyKiTucXa.API.DTOs;

/// <summary>
/// DTO for User - used for API responses (excludes sensitive data)
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? ProfileImage { get; set; }
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new User
/// </summary>
public class CreateUserDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 255 characters")]
    public string FullName { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }

    [Url(ErrorMessage = "Invalid profile image URL")]
    public string? ProfileImage { get; set; }

    [RegularExpression("^(Admin|Manager|Staff|User)$", ErrorMessage = "Role must be Admin, Manager, Staff, or User")]
    public string Role { get; set; } = "User";
}

/// <summary>
/// DTO for updating an existing User
/// </summary>
public class UpdateUserDto
{
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 255 characters")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }

    [Url(ErrorMessage = "Invalid profile image URL")]
    public string? ProfileImage { get; set; }

    [RegularExpression("^(Admin|Manager|Staff|User)$", ErrorMessage = "Role must be Admin, Manager, Staff, or User")]
    public string? Role { get; set; }

    public bool? IsActive { get; set; }
}
