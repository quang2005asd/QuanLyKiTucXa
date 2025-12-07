using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

public class RoomsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RoomsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<RoomDto>>> GetRooms(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<RoomDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Rooms.CountAsync();
        var rooms = await _context.Rooms
            .Include(r => r.Floor)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var roomDtos = _mapper.Map<List<RoomDto>>(rooms);
        return OkPaginatedResponse(roomDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<RoomDto>>> GetRoom(int id)
    {
        var room = await _context.Rooms.Include(r => r.Floor).FirstOrDefaultAsync(r => r.Id == id);
        if (room == null)
            return NotFoundResponse<RoomDto>("Room not found");

        var roomDto = _mapper.Map<RoomDto>(room);
        return OkResponse(roomDto);
    }

    [HttpGet("building/{buildingId}")]
    public async Task<ActionResult<PaginatedResponseDto<RoomDto>>> GetRoomsByBuilding(int buildingId, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<RoomDto>("Page number and page size must be greater than 0");

        // Check if building exists
        var buildingExists = await _context.Buildings.AnyAsync(b => b.Id == buildingId);
        if (!buildingExists)
            return NotFoundPaginatedResponse<RoomDto>("Building not found");

        var totalCount = await _context.Rooms
            .Where(r => r.Floor.BuildingId == buildingId)
            .CountAsync();

        var rooms = await _context.Rooms
            .Where(r => r.Floor.BuildingId == buildingId)
            .Include(r => r.Floor)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var roomDtos = _mapper.Map<List<RoomDto>>(rooms);
        return OkPaginatedResponse(roomDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("available")]
    public async Task<ActionResult<PaginatedResponseDto<RoomDto>>> GetAvailableRooms(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<RoomDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Rooms
            .Where(r => r.Status == "Available")
            .CountAsync();

        var rooms = await _context.Rooms
            .Where(r => r.Status == "Available")
            .Include(r => r.Floor)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var roomDtos = _mapper.Map<List<RoomDto>>(rooms);
        return OkPaginatedResponse(roomDtos, totalCount, pageNumber, pageSize);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<RoomDto>>> PostRoom(CreateRoomDto createRoomDto)
    {
        var validationError = ValidateModelState<RoomDto>();
        if (validationError != null)
            return validationError;

        // Check if floor exists
        var floorExists = await _context.Floors.AnyAsync(f => f.Id == createRoomDto.FloorId);
        if (!floorExists)
            return BadRequestResponse<RoomDto>("Floor not found");

        // Check if room number already exists in the floor
        if (await _context.Rooms.AnyAsync(r => r.RoomNumber == createRoomDto.RoomNumber && r.FloorId == createRoomDto.FloorId))
            return BadRequestResponse<RoomDto>("Room number already exists in this floor");

        var room = _mapper.Map<Room>(createRoomDto);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var roomDto = _mapper.Map<RoomDto>(room);
        return CreatedResponse("GetRoom", new { id = room.Id }, roomDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<RoomDto>>> PutRoom(int id, UpdateRoomDto updateRoomDto)
    {
        var validationError = ValidateModelState<RoomDto>();
        if (validationError != null)
            return validationError;

        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFoundResponse<RoomDto>("Room not found");

        _mapper.Map(updateRoomDto, room);
        room.UpdatedAt = DateTime.UtcNow;

        try
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RoomExists(id))
                return NotFoundResponse<RoomDto>("Room not found");
            throw;
        }

        var roomDto = _mapper.Map<RoomDto>(room);
        return OkResponse(roomDto, "Room updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return NotFoundResponse<object>("Room not found");

        // Soft delete
        room.IsDeleted = true;
        room.DeletedAt = DateTime.UtcNow;
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();

        return OkResponse<object>(new { }, "Room deleted successfully");
    }

    private bool RoomExists(int id)
    {
        return _context.Rooms.Any(e => e.Id == id);
    }
}
