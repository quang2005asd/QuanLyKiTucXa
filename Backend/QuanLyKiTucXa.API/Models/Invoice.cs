namespace QuanLyKiTucXa.API.Models;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public int ContractId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal RentAmount { get; set; }
    public decimal ServiceAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Unpaid"; // Unpaid, Paid, Overdue, Cancelled
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation property
    public Contract Contract { get; set; } = null!;
}
