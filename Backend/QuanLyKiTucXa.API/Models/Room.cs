namespace QuanLyKiTucXa.API.Models;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = null!;
    public int FloorId { get; set; }
    public int Capacity { get; set; }
    public decimal RentPrice { get; set; }
    public string Status { get; set; } = "Available"; // Available, Occupied, Maintenance
    public string RoomType { get; set; } = "Single"; // Single, Double, Triple, Quad
    public bool HasAirConditioner { get; set; } = false;
    public bool HasWaterHeater { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public Floor Floor { get; set; } = null!;
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
