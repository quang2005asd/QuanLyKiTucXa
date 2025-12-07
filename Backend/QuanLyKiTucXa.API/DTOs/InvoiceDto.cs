using System.ComponentModel.DataAnnotations;

namespace QuanLyKiTucXa.API.DTOs;

/// <summary>
/// DTO for Invoice - used for API responses
/// </summary>
public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public int ContractId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal RentAmount { get; set; }
    public decimal ServiceAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Unpaid";
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Invoice
/// </summary>
public class CreateInvoiceDto
{
    [Required(ErrorMessage = "Invoice number is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Invoice number must be between 1 and 50 characters")]
    public string InvoiceNumber { get; set; } = null!;

    [Required(ErrorMessage = "Contract ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Contract ID must be greater than 0")]
    public int ContractId { get; set; }

    [Required(ErrorMessage = "Month is required")]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
    public int Month { get; set; }

    [Required(ErrorMessage = "Year is required")]
    [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Rent amount is required")]
    [Range(0, 999999.99, ErrorMessage = "Rent amount must be greater than or equal to 0")]
    public decimal RentAmount { get; set; }

    [Required(ErrorMessage = "Service amount is required")]
    [Range(0, 999999.99, ErrorMessage = "Service amount must be greater than or equal to 0")]
    public decimal ServiceAmount { get; set; }

    [Required(ErrorMessage = "Due date is required")]
    public DateTime DueDate { get; set; }
}

/// <summary>
/// DTO for updating an existing Invoice
/// </summary>
public class UpdateInvoiceDto
{
    [RegularExpression("^(Unpaid|Paid|Overdue|Cancelled)$", ErrorMessage = "Status must be Unpaid, Paid, Overdue, or Cancelled")]
    public string? Status { get; set; }

    public DateTime? PaymentDate { get; set; }
}
