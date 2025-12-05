namespace Backend_API.Models
{
    public class Building
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ví dụ: Nhà A1
        public int TotalFloors { get; set; }
    }
}