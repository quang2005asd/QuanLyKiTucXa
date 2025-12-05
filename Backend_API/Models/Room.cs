using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_API.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty; 

        public int Floor { get; set; } 

        public string Type { get; set; } = "Standard"; 
        public int MaxCapacity { get; set; } = 4;
        public decimal PricePerMonth { get; set; }
        public string Status { get; set; } = "Trống";

        public int BuildingId { get; set; }
        [ForeignKey("BuildingId")]
        public Building? Building { get; set; }
    }
}