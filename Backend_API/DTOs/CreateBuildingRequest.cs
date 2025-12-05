namespace Backend_API.DTOs
{
    public class CreateBuildingRequest
    {
        public string Name { get; set; } = string.Empty; 
        public int TotalFloors { get; set; }          
        public int RoomsPerFloor { get; set; }          
    }
}