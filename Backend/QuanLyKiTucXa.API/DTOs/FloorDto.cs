using System.ComponentModel.DataAnnotations;

namespace QuanLyKiTucXa.API.DTOs;

/// <summary>
/// DTO for Floor - used for API responses
/// </summary>
public class FloorDto
{
    public int Id { get; set; }
    public int FloorNumber { get; set; }
    public int BuildingId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Floor
/// </summary>
public class CreateFloorDto
{
    [Required(ErrorMessage = "Floor number is required")]
    [Range(1, 100, ErrorMessage = "Floor number must be between 1 and 100")]
    public int FloorNumber { get; set; }

    [Required(ErrorMessage = "Building ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Building ID must be greater than 0")]
    public int BuildingId { get; set; }

    [StringLength(500, ErrorMessage = "Description must be at most 500 characters")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating an existing Floor
/// </summary>
public class UpdateFloorDto
{
    [Range(1, 100, ErrorMessage = "Floor number must be between 1 and 100")]
    public int? FloorNumber { get; set; }

    [StringLength(500, ErrorMessage = "Description must be at most 500 characters")]
    public string? Description { get; set; }
}
