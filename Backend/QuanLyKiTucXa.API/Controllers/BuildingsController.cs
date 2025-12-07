using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BuildingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Building>>> GetBuildings()
    {
        return await _context.Buildings.Include(b => b.Floors).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Building>> GetBuilding(int id)
    {
        var building = await _context.Buildings.Include(b => b.Floors).FirstOrDefaultAsync(b => b.Id == id);
        if (building == null)
        {
            return NotFound();
        }
        return building;
    }

    [HttpPost]
    public async Task<ActionResult<Building>> PostBuilding(Building building)
    {
        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetBuilding", new { id = building.Id }, building);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutBuilding(int id, Building building)
    {
        if (id != building.Id)
        {
            return BadRequest();
        }

        _context.Entry(building).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BuildingExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBuilding(int id)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building == null)
        {
            return NotFound();
        }

        _context.Buildings.Remove(building);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BuildingExists(int id)
    {
        return _context.Buildings.Any(e => e.Id == id);
    }
}
