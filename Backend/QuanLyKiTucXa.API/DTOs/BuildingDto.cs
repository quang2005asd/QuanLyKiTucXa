using System.ComponentModel.DataAnnotations;

namespace QuanLyKiTucXa.API.DTOs;

/// <summary>
/// DTO for Building - used for API responses
/// </summary>
public class BuildingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int TotalFloors { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Building
/// </summary>
public class CreateBuildingDto
{
    [Required(ErrorMessage = "Building name is required")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Building name must be between 2 and 255 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Total floors is required")]
    [Range(1, 50, ErrorMessage = "Total floors must be between 1 and 50")]
    public int TotalFloors { get; set; }

    [StringLength(1000, ErrorMessage = "Description must be at most 1000 characters")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating an existing Building
/// </summary>
public class UpdateBuildingDto
{
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Building name must be between 2 and 255 characters")]
    public string? Name { get; set; }

    [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters")]
    public string? Address { get; set; }

    [Range(1, 50, ErrorMessage = "Total floors must be between 1 and 50")]
    public int? TotalFloors { get; set; }

    [StringLength(1000, ErrorMessage = "Description must be at most 1000 characters")]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}
