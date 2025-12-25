namespace QuanLyKiTucXa.API.Models;

/// <summary>
/// Join table for many-to-many relationship between Contract and Student
/// One contract can have multiple students
/// </summary>
public class ContractStudent
{
    public int ContractId { get; set; }
    public int StudentId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Contract Contract { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
