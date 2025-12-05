using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_API.Data;
using Backend_API.Models;
using Backend_API.DTOs; // Nhớ dòng này để dùng DTO vừa tạo

namespace Backend_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BuildingsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách Tòa nhà
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuildings()
        {
            return await _context.Buildings.ToListAsync();
        }

        // 2. API "COMBO": Tạo Tòa nhà + Tự động sinh phòng
        [HttpPost]
        public async Task<ActionResult<Building>> CreateBuilding(CreateBuildingRequest request)
        {
            // BƯỚC 1: Tạo và Lưu Tòa nhà trước
            var building = new Building
            {
                Name = request.Name,
                TotalFloors = request.TotalFloors
            };

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync(); // Lưu để lấy được cái ID (building.Id)

            // BƯỚC 2: Chạy vòng lặp tạo phòng tự động
            var newRooms = new List<Room>();

            for (int f = 1; f <= request.TotalFloors; f++)
            {
                for (int r = 1; r <= request.RoomsPerFloor; r++)
                {
                    // Tạo số phòng kiểu: 101... 120, 201... 220
                    string roomNum = (f * 100 + r).ToString();

                    newRooms.Add(new Room
                    {
                        RoomNumber = roomNum,
                        Floor = f,
                        BuildingId = building.Id, // Gắn phòng vào tòa vừa tạo
                        Type = "Standard",
                        MaxCapacity = 4,      // Mặc định 4 người
                        PricePerMonth = 500000, // Mặc định 500k
                        Status = "Trống"
                    });
                }
            }
            await _context.Rooms.AddRangeAsync(newRooms);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBuildings", new { id = building.Id }, building);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuilding(int id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null) return NotFound();

            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}