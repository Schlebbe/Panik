namespace Panik.Models
{
    public class TaxaDTO
    {
        public static readonly Parking Taxa1 = new Parking
        {
            WeekDaysTime = "00-24",
            WeekDaysPrice = 55,
            WeekendTime = "00-24",
            WeekendPrice = 55,
            OtherTimePrice = 55,
        };
        
        public static readonly Parking Taxa2 = new Parking 
        { 
            WeekDaysTime = "07-21", 
            WeekDaysPrice = 31, 
            WeekendTime = "09-19",
            WeekendPrice = 31,
            OtherTimePrice = 20,
        };
        public static readonly Parking Taxa3 = new Parking 
        { 
            WeekDaysTime = "07-19", 
            WeekDaysPrice = 20, 
            WeekendTime = "11-17", 
            WeekendPrice = 15,
            OtherTimePrice = 0,
        };
        public static readonly Parking Taxa4 = new Parking 
        { 
            WeekDaysTime = "07-19", 
            WeekDaysPrice = 10, 
            WeekendTime = "11-17", 
            WeekendPrice = 10,
            OtherTimePrice = 0, 
        };
        public static readonly Parking Taxa5 = new Parking 
        { 
            WeekDaysTime = "07-19", 
            WeekDaysPrice = 5,
            WeekendTime = "",
            WeekendPrice = 0,
            OtherTimePrice = 0, 
        };

        public static readonly List<Parking> AllTaxa = new List<Parking>
        {
            Taxa1,
            Taxa2,
            Taxa3,
            Taxa4,
            Taxa5
        };
    }
}
