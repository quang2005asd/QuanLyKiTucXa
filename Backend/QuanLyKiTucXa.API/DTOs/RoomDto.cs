using System.ComponentModel.DataAnnotations;

namespace QuanLyKiTucXa.API.DTOs;

/// <summary>
/// DTO for Room - used for API responses
/// </summary>
public class RoomDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = null!;
    public int FloorId { get; set; }
    public int Capacity { get; set; }
    public decimal RentPrice { get; set; }
    public string Status { get; set; } = "Available";
    public string RoomType { get; set; } = "Single";
    public bool HasAirConditioner { get; set; }
    public bool HasWaterHeater { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Room
/// </summary>
public class CreateRoomDto
{
    [Required(ErrorMessage = "Room number is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Room number must be between 1 and 50 characters")]
    public string RoomNumber { get; set; } = null!;

    [Required(ErrorMessage = "Floor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Floor ID must be greater than 0")]
    public int FloorId { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Rent price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Rent price must be greater than 0")]
    public decimal RentPrice { get; set; }

    [RegularExpression("^(Single|Double|Triple|Quad)$", ErrorMessage = "Room type must be Single, Double, Triple, or Quad")]
    public string RoomType { get; set; } = "Single";

    public bool HasAirConditioner { get; set; } = false;
    public bool HasWaterHeater { get; set; } = false;
}

/// <summary>
/// DTO for updating an existing Room
/// </summary>
public class UpdateRoomDto
{
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Room number must be between 1 and 50 characters")]
    public string? RoomNumber { get; set; }

    [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10")]
    public int? Capacity { get; set; }

    [Range(0.01, 999999.99, ErrorMessage = "Rent price must be greater than 0")]
    public decimal? RentPrice { get; set; }

    [RegularExpression("^(Available|Occupied|Maintenance)$", ErrorMessage = "Status must be Available, Occupied, or Maintenance")]
    public string? Status { get; set; }

    [RegularExpression("^(Single|Double|Triple|Quad)$", ErrorMessage = "Room type must be Single, Double, Triple, or Quad")]
    public string? RoomType { get; set; }

    public bool? HasAirConditioner { get; set; }
    public bool? HasWaterHeater { get; set; }
}
