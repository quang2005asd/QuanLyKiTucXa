using System.ComponentModel.DataAnnotations;

namespace QuanLyKiTucXa.API.DTOs;

/// <summary>
/// DTO for Contract - used for API responses
/// </summary>
public class ContractDto
{
    public int Id { get; set; }
    public string ContractNumber { get; set; } = null!;
    public int StudentId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal MonthlyRent { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Contract
/// </summary>
public class CreateContractDto
{
    [Required(ErrorMessage = "Contract number is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Contract number must be between 1 and 50 characters")]
    public string ContractNumber { get; set; } = null!;

    [Required(ErrorMessage = "Student ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Student ID must be greater than 0")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Room ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Room ID must be greater than 0")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Deposit amount is required")]
    [Range(0, 999999.99, ErrorMessage = "Deposit amount must be greater than or equal to 0")]
    public decimal DepositAmount { get; set; }

    [Required(ErrorMessage = "Monthly rent is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Monthly rent must be greater than 0")]
    public decimal MonthlyRent { get; set; }
}

/// <summary>
/// DTO for updating an existing Contract
/// </summary>
public class UpdateContractDto
{
    public DateTime? EndDate { get; set; }

    [RegularExpression("^(Active|Completed|Cancelled|Pending)$", ErrorMessage = "Status must be Active, Completed, Cancelled, or Pending")]
    public string? Status { get; set; }
}
