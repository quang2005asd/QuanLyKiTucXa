using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

public class FloorsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public FloorsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<FloorDto>>> GetFloors(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<FloorDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Floors.CountAsync();
        var floors = await _context.Floors
            .Include(f => f.Building)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var floorDtos = _mapper.Map<List<FloorDto>>(floors);
        return OkPaginatedResponse(floorDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<FloorDto>>> GetFloor(int id)
    {
        var floor = await _context.Floors.Include(f => f.Building).FirstOrDefaultAsync(f => f.Id == id);
        if (floor == null)
            return NotFoundResponse<FloorDto>("Floor not found");

        var floorDto = _mapper.Map<FloorDto>(floor);
        return OkResponse(floorDto);
    }

    [HttpGet("building/{buildingId}")]
    public async Task<ActionResult<PaginatedResponseDto<FloorDto>>> GetFloorsByBuilding(int buildingId, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<FloorDto>("Page number and page size must be greater than 0");

        // Check if building exists
        var buildingExists = await _context.Buildings.AnyAsync(b => b.Id == buildingId);
        if (!buildingExists)
            return NotFoundPaginatedResponse<FloorDto>("Building not found");

        var totalCount = await _context.Floors
            .Where(f => f.BuildingId == buildingId)
            .CountAsync();

        var floors = await _context.Floors
            .Where(f => f.BuildingId == buildingId)
            .Include(f => f.Building)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var floorDtos = _mapper.Map<List<FloorDto>>(floors);
        return OkPaginatedResponse(floorDtos, totalCount, pageNumber, pageSize);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<FloorDto>>> PostFloor(CreateFloorDto createFloorDto)
    {
        var validationError = ValidateModelState<FloorDto>();
        if (validationError != null)
            return validationError;

        // Check if building exists
        var buildingExists = await _context.Buildings.AnyAsync(b => b.Id == createFloorDto.BuildingId);
        if (!buildingExists)
            return BadRequestResponse<FloorDto>("Building not found");

        var floor = _mapper.Map<Floor>(createFloorDto);
        _context.Floors.Add(floor);
        await _context.SaveChangesAsync();

        var floorDto = _mapper.Map<FloorDto>(floor);
        return CreatedResponse("GetFloor", new { id = floor.Id }, floorDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<FloorDto>>> PutFloor(int id, UpdateFloorDto updateFloorDto)
    {
        var validationError = ValidateModelState<FloorDto>();
        if (validationError != null)
            return validationError;

        var floor = await _context.Floors.FindAsync(id);
        if (floor == null)
            return NotFoundResponse<FloorDto>("Floor not found");

        _mapper.Map(updateFloorDto, floor);
        floor.UpdatedAt = DateTime.UtcNow;

        try
        {
            _context.Floors.Update(floor);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FloorExists(id))
                return NotFoundResponse<FloorDto>("Floor not found");
            throw;
        }

        var floorDto = _mapper.Map<FloorDto>(floor);
        return OkResponse(floorDto, "Floor updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteFloor(int id)
    {
        var floor = await _context.Floors.FindAsync(id);
        if (floor == null)
            return NotFoundResponse<object>("Floor not found");

        // Soft delete
        floor.IsDeleted = true;
        floor.DeletedAt = DateTime.UtcNow;
        _context.Floors.Update(floor);
        await _context.SaveChangesAsync();

        return OkResponse<object>(new { }, "Floor deleted successfully");
    }

    private bool FloorExists(int id)
    {
        return _context.Floors.Any(e => e.Id == id);
    }
}
