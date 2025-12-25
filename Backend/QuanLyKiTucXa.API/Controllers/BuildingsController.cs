using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

public class BuildingsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BuildingsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<BuildingDto>>> GetBuildings(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<BuildingDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Buildings.CountAsync();
        var buildings = await _context.Buildings
            .Include(b => b.Floors)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var buildingDtos = _mapper.Map<List<BuildingDto>>(buildings);
        return OkPaginatedResponse(buildingDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<BuildingDto>>> GetBuilding(int id)
    {
        var building = await _context.Buildings.Include(b => b.Floors).FirstOrDefaultAsync(b => b.Id == id);
        if (building == null)
            return NotFoundResponse<BuildingDto>("Building not found");

        var buildingDto = _mapper.Map<BuildingDto>(building);
        return OkResponse(buildingDto);
    }

    [HttpPost]
    // [Authorize(Policy = "ManagerOrAdmin")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponseDto<BuildingDto>>> PostBuilding(CreateBuildingDto createBuildingDto)
    {
        var validationError = ValidateModelState<BuildingDto>();
        if (validationError != null)
            return validationError;

        var building = _mapper.Map<Building>(createBuildingDto);
        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();

        var buildingDto = _mapper.Map<BuildingDto>(building);
        return CreatedResponse("GetBuilding", new { id = building.Id }, buildingDto);
    }

    [HttpPut("{id}")]
    // [Authorize(Policy = "ManagerOrAdmin")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponseDto<BuildingDto>>> PutBuilding(int id, UpdateBuildingDto updateBuildingDto)
    {
        var validationError = ValidateModelState<BuildingDto>();
        if (validationError != null)
            return validationError;

        var building = await _context.Buildings.FindAsync(id);
        if (building == null)
            return NotFoundResponse<BuildingDto>("Building not found");

        _mapper.Map(updateBuildingDto, building);
        building.UpdatedAt = DateTime.UtcNow;

        try
        {
            _context.Buildings.Update(building);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BuildingExists(id))
                return NotFoundResponse<BuildingDto>("Building not found");
            throw;
        }

        var buildingDto = _mapper.Map<BuildingDto>(building);
        return OkResponse(buildingDto, "Building updated successfully");
    }

    [HttpDelete("{id}")]
    // [Authorize(Policy = "AdminOnly")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteBuilding(int id)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building == null)
            return NotFoundResponse<object>("Building not found");

        // Soft delete
        building.IsDeleted = true;
        building.DeletedAt = DateTime.UtcNow;
        _context.Buildings.Update(building);
        await _context.SaveChangesAsync();

        return OkResponse<object>(new { }, "Building deleted successfully");
    }

    private bool BuildingExists(int id)
    {
        return _context.Buildings.Any(e => e.Id == id);
    }
}
