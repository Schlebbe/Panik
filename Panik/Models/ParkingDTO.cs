namespace Panik.Models
{
    public class ParkingDTO
    {
        public bool CanPark { get; set; }
        public bool PaidParking { get; set; }
        public int? Price { get; set; }
        public string TimeInterval { get; set; }
        public string Address { get; set; }
    }
}
