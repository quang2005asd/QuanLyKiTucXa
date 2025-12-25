namespace QuanLyKiTucXa.API.Models;

public class Contract
{
    public int Id { get; set; }
    public string ContractNumber { get; set; } = null!;
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal MonthlyRent { get; set; }
    public string Status { get; set; } = "Active"; // Active, Completed, Cancelled, Pending
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public Room Room { get; set; } = null!;
    public ICollection<ContractStudent> ContractStudents { get; set; } = new List<ContractStudent>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
