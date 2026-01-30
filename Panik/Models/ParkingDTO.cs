namespace Panik.Models
{
    public class ParkingDTO
    {
        public string WeekDaysTime { get; set; }
        public int? WeekDaysPrice { get; set; }
        public string WeekendTime { get; set; }
        public int? OtherDayPrice { get; set; }
    }
}
