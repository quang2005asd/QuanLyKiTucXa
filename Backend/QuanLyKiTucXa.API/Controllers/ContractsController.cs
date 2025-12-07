using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

public class ContractsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ContractsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<ContractDto>>> GetContracts(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<ContractDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Contracts.CountAsync();
        var contracts = await _context.Contracts
            .Include(c => c.Student)
            .Include(c => c.Room)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var contractDtos = _mapper.Map<List<ContractDto>>(contracts);
        return OkPaginatedResponse(contractDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<ContractDto>>> GetContract(int id)
    {
        var contract = await _context.Contracts
            .Include(c => c.Student)
            .Include(c => c.Room)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contract == null)
            return NotFoundResponse<ContractDto>("Contract not found");

        var contractDto = _mapper.Map<ContractDto>(contract);
        return OkResponse(contractDto);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<PaginatedResponseDto<ContractDto>>> GetContractsByStudent(int studentId, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<ContractDto>("Page number and page size must be greater than 0");

        // Check if student exists
        var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
        if (!studentExists)
            return NotFoundPaginatedResponse<ContractDto>("Student not found");

        var totalCount = await _context.Contracts
            .Where(c => c.StudentId == studentId)
            .CountAsync();

        var contracts = await _context.Contracts
            .Where(c => c.StudentId == studentId)
            .Include(c => c.Student)
            .Include(c => c.Room)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var contractDtos = _mapper.Map<List<ContractDto>>(contracts);
        return OkPaginatedResponse(contractDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("room/{roomId}")]
    public async Task<ActionResult<PaginatedResponseDto<ContractDto>>> GetContractsByRoom(int roomId, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<ContractDto>("Page number and page size must be greater than 0");

        // Check if room exists
        var roomExists = await _context.Rooms.AnyAsync(r => r.Id == roomId);
        if (!roomExists)
            return NotFoundPaginatedResponse<ContractDto>("Room not found");

        var totalCount = await _context.Contracts
            .Where(c => c.RoomId == roomId && c.Status == "Active")
            .CountAsync();

        var contracts = await _context.Contracts
            .Where(c => c.RoomId == roomId && c.Status == "Active")
            .Include(c => c.Student)
            .Include(c => c.Room)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var contractDtos = _mapper.Map<List<ContractDto>>(contracts);
        return OkPaginatedResponse(contractDtos, totalCount, pageNumber, pageSize);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ContractDto>>> PostContract(CreateContractDto createContractDto)
    {
        var validationError = ValidateModelState<ContractDto>();
        if (validationError != null)
            return validationError;

        // Check if student exists
        var student = await _context.Students.FindAsync(createContractDto.StudentId);
        if (student == null)
            return BadRequestResponse<ContractDto>("Student not found");

        // Check if room exists
        var room = await _context.Rooms.FindAsync(createContractDto.RoomId);
        if (room == null)
            return BadRequestResponse<ContractDto>("Room not found");

        // Check if room is available
        if (room.Status != "Available")
            return BadRequestResponse<ContractDto>("Room is not available");

        // Check if contract number already exists
        if (await _context.Contracts.AnyAsync(c => c.ContractNumber == createContractDto.ContractNumber))
            return BadRequestResponse<ContractDto>("Contract number already exists");

        // Validate dates
        if (createContractDto.StartDate >= createContractDto.EndDate)
            return BadRequestResponse<ContractDto>("Start date must be before end date");

        var contract = _mapper.Map<Contract>(createContractDto);
        contract.CreatedAt = DateTime.UtcNow;

        // Update Room status to Occupied
        room.Status = "Occupied";

        _context.Contracts.Add(contract);
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();

        var contractDto = _mapper.Map<ContractDto>(contract);
        return CreatedResponse("GetContract", new { id = contract.Id }, contractDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<ContractDto>>> PutContract(int id, UpdateContractDto updateContractDto)
    {
        var validationError = ValidateModelState<ContractDto>();
        if (validationError != null)
            return validationError;

        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
            return NotFoundResponse<ContractDto>("Contract not found");

        _mapper.Map(updateContractDto, contract);
        contract.UpdatedAt = DateTime.UtcNow;

        try
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ContractExists(id))
                return NotFoundResponse<ContractDto>("Contract not found");
            throw;
        }

        var contractDto = _mapper.Map<ContractDto>(contract);
        return OkResponse(contractDto, "Contract updated successfully");
    }

    [HttpPut("{id}/complete")]
    public async Task<ActionResult<ApiResponseDto<ContractDto>>> CompleteContract(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
            return NotFoundResponse<ContractDto>("Contract not found");

        contract.Status = "Completed";
        contract.UpdatedAt = DateTime.UtcNow;

        // Update Room status back to Available
        var room = await _context.Rooms.FindAsync(contract.RoomId);
        if (room != null)
        {
            room.Status = "Available";
            room.UpdatedAt = DateTime.UtcNow;
            _context.Rooms.Update(room);
        }

        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();

        var contractDto = _mapper.Map<ContractDto>(contract);
        return OkResponse(contractDto, "Contract completed successfully");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteContract(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
            return NotFoundResponse<object>("Contract not found");

        // Soft delete
        contract.IsDeleted = true;
        contract.DeletedAt = DateTime.UtcNow;
        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();

        return OkResponse<object>(new { }, "Contract deleted successfully");
    }

    private bool ContractExists(int id)
    {
        return _context.Contracts.Any(e => e.Id == id);
    }
}
