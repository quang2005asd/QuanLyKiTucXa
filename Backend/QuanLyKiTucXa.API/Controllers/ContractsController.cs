using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ContractsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContracts()
    {
        return await _context.Contracts
            .Include(c => c.Student)
            .Include(c => c.Room)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Contract>> GetContract(int id)
    {
        var contract = await _context.Contracts
            .Include(c => c.Student)
            .Include(c => c.Room)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contract == null)
        {
            return NotFound();
        }
        return contract;
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContractsByStudent(int studentId)
    {
        return await _context.Contracts
            .Where(c => c.StudentId == studentId)
            .Include(c => c.Student)
            .Include(c => c.Room)
            .ToListAsync();
    }

    [HttpGet("room/{roomId}")]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContractsByRoom(int roomId)
    {
        return await _context.Contracts
            .Where(c => c.RoomId == roomId && c.Status == "Active")
            .Include(c => c.Student)
            .Include(c => c.Room)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Contract>> PostContract(Contract contract)
    {
        // Generate ContractNumber
        contract.ContractNumber = $"CT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";

        // Update Room status to Occupied
        var room = await _context.Rooms.FindAsync(contract.RoomId);
        if (room != null)
        {
            room.Status = "Occupied";
        }

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetContract", new { id = contract.Id }, contract);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutContract(int id, Contract contract)
    {
        if (id != contract.Id)
        {
            return BadRequest();
        }

        _context.Entry(contract).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ContractExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteContract(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
        {
            return NotFound();
        }

        contract.Status = "Completed";

        // Update Room status back to Available
        var room = await _context.Rooms.FindAsync(contract.RoomId);
        if (room != null)
        {
            room.Status = "Available";
        }

        _context.Entry(contract).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
        {
            return NotFound();
        }

        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ContractExists(int id)
    {
        return _context.Contracts.Any(e => e.Id == id);
    }
}
