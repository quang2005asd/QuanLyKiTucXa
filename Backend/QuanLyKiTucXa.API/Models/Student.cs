namespace QuanLyKiTucXa.API.Models;

public class Student
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
    public string Status { get; set; } = "Active"; // Active, Inactive, Graduated
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation property
    public ICollection<ContractStudent> ContractStudents { get; set; } = new List<ContractStudent>();
}
