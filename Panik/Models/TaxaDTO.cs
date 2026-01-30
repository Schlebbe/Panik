namespace Panik.Models
{
    public class TaxaDTO
    {
        public static readonly ParkingDTO Taxa1 = new ParkingDTO
        {
            WeekDaysTime = "00-24",
            WeekDaysPrice = 55,
            WeekendTime = "00-24",
            WeekendPrice = 55,
            OtherTimePrice = 55,
        };
        
        public static readonly ParkingDTO Taxa2 = new ParkingDTO 
        { 
            WeekDaysTime = "07-21", 
            WeekDaysPrice = 31, 
            WeekendTime = "09-19",
            WeekendPrice = 31,
            OtherTimePrice = 20,
        };
        public static readonly ParkingDTO Taxa3 = new ParkingDTO 
        { 
            WeekDaysTime = "07-19", 
            WeekDaysPrice = 20, 
            WeekendTime = "11-17", 
            WeekendPrice = 15,
            OtherTimePrice = 0,
        };
        public static readonly ParkingDTO Taxa4 = new ParkingDTO 
        { 
            WeekDaysTime = "07-19", 
            WeekDaysPrice = 10, 
            WeekendTime = "11-17", 
            WeekendPrice = 10,
            OtherTimePrice = 0, 
        };
        public static readonly ParkingDTO Taxa5 = new ParkingDTO 
        { 
            WeekDaysTime = "07-19", 
            WeekDaysPrice = 5,
            WeekendTime = "",
            WeekendPrice = 0,
            OtherTimePrice = 0, 
        };

        public static readonly List<ParkingDTO> AllTaxa = new List<ParkingDTO>
        {
            Taxa1,
            Taxa2,
            Taxa3,
            Taxa4,
            Taxa5
        };
    }
}
