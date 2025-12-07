using System.ComponentModel.DataAnnotations;

namespace QuanLyKiTucXa.API.DTOs;

/// <summary>
/// DTO for Student - used for API responses
/// </summary>
public class StudentDto
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? IdCardNumber { get; set; }
    public string? Address { get; set; }
    public string? Guardian { get; set; }
    public string? GuardianPhone { get; set; }
    public string? ProfileImage { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Student
/// </summary>
public class CreateStudentDto
{
    [Required(ErrorMessage = "Student code is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Student code must be between 1 and 50 characters")]
    public string StudentCode { get; set; } = null!;

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 255 characters")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(20, ErrorMessage = "ID card number must be at most 20 characters")]
    public string? IdCardNumber { get; set; }

    [StringLength(500, ErrorMessage = "Address must be at most 500 characters")]
    public string? Address { get; set; }

    [StringLength(255, ErrorMessage = "Guardian name must be at most 255 characters")]
    public string? Guardian { get; set; }

    [Phone(ErrorMessage = "Invalid guardian phone number")]
    public string? GuardianPhone { get; set; }

    [Url(ErrorMessage = "Invalid profile image URL")]
    public string? ProfileImage { get; set; }
}

/// <summary>
/// DTO for updating an existing Student
/// </summary>
public class UpdateStudentDto
{
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 255 characters")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(20, ErrorMessage = "ID card number must be at most 20 characters")]
    public string? IdCardNumber { get; set; }

    [StringLength(500, ErrorMessage = "Address must be at most 500 characters")]
    public string? Address { get; set; }

    [StringLength(255, ErrorMessage = "Guardian name must be at most 255 characters")]
    public string? Guardian { get; set; }

    [Phone(ErrorMessage = "Invalid guardian phone number")]
    public string? GuardianPhone { get; set; }

    [Url(ErrorMessage = "Invalid profile image URL")]
    public string? ProfileImage { get; set; }

    [RegularExpression("^(Active|Inactive|Graduated)$", ErrorMessage = "Status must be Active, Inactive, or Graduated")]
    public string? Status { get; set; }
}
