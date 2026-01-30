using Panik.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Panik.Endpoints
{
    public class ParkingEndpoints
    {
        public static string APIKey { get; set; } = string.Empty;
        public static void MapParkingEndpoints(WebApplication app)
        {
            app.MapGet("/parking", async (HttpContext httpContext, HttpClient httpClient, string lat, string lng, int radius = 100) =>
            {
                if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lng))
                {
                    return Results.BadRequest("Invalid input parameters.");
                }

                string url = $"https://openparking.stockholm.se/LTF-Tolken/v1/ptillaten/within?radius={radius}&lat={lat}&lng={lng}&maxFeatures=50&outputFormat=json&apiKey={APIKey}";

                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<ParkingResponseData>(content);
                if (data == null || data.Features.Count == 0)
                {
                    return Results.NotFound("No parking data found for the given location.");
                }

                var parkingRate = data.Features[0].Properties.ParkingRate;
                var match = Regex.Match(parkingRate, @"\b[Tt]axa ([1-5])\b");

                if (!match.Success)
                {
                    return Results.BadRequest("Format is invalid.");
                }

                int.TryParse(match.Groups[1].Value, out var number);

                var currentTime = DateTime.Now;
                var currentDay = currentTime.DayOfWeek;

                var parkingInfo = new ParkingDTO
                {
                    CanPark = true, //TODO: Add logic for canPark based on other rules
                    Address = data.Features[0].Properties.Address
                };

                switch (number)
                {
                    case 1:
                        parkingInfo.PaidParking = true;
                        parkingInfo.Price = TaxaDTO.Taxa1.WeekDaysPrice;
                        parkingInfo.TimeInterval = TaxaDTO.Taxa1.WeekDaysTime;
                        break;
                    case 2:
                        if (currentDay == DayOfWeek.Saturday || currentDay == DayOfWeek.Sunday)
                        {
                            parkingInfo.PaidParking = CalculateParkingHours(currentTime, TaxaDTO.Taxa2.WeekendTime);
                            parkingInfo.TimeInterval = TaxaDTO.Taxa2.WeekendTime;
                            parkingInfo.Price = TaxaDTO.Taxa2.WeekendPrice;
                        }
                        else
                        {
                            parkingInfo.PaidParking = CalculateParkingHours(currentTime, TaxaDTO.Taxa2.WeekDaysTime);
                            parkingInfo.TimeInterval = TaxaDTO.Taxa2.WeekDaysTime;
                            parkingInfo.Price = TaxaDTO.Taxa2.WeekDaysPrice;
                        }
                        break;
                    case 3:
                        if (currentDay == DayOfWeek.Saturday)
                        {
                            parkingInfo.PaidParking = CalculateParkingHours(currentTime, TaxaDTO.Taxa3.WeekendTime);
                            parkingInfo.TimeInterval = TaxaDTO.Taxa3.WeekendTime;
                            parkingInfo.Price = TaxaDTO.Taxa3.WeekendPrice;
                        }
                        else
                        {
                            parkingInfo.PaidParking = CalculateParkingHours(currentTime, TaxaDTO.Taxa3.WeekDaysTime);
                            parkingInfo.TimeInterval = TaxaDTO.Taxa3.WeekDaysTime;
                            parkingInfo.Price = TaxaDTO.Taxa3.WeekDaysPrice;
                        }
                        parkingInfo.PaidParking = currentDay == DayOfWeek.Sunday ? false : parkingInfo.PaidParking;
                        break;
                    case 4:
                        if (currentDay == DayOfWeek.Saturday)
                        {
                            parkingInfo.PaidParking = CalculateParkingHours(currentTime, TaxaDTO.Taxa4.WeekendTime);
                            parkingInfo.TimeInterval = TaxaDTO.Taxa4.WeekendTime;
                            parkingInfo.Price = TaxaDTO.Taxa4.WeekendPrice;
                        }
                        else
                        {
                            parkingInfo.PaidParking = CalculateParkingHours(currentTime, TaxaDTO.Taxa4.WeekDaysTime);
                            parkingInfo.TimeInterval = TaxaDTO.Taxa4.WeekDaysTime;
                            parkingInfo.Price = TaxaDTO.Taxa4.WeekDaysPrice;
                        }
                        parkingInfo.PaidParking = currentDay == DayOfWeek.Sunday ? false : parkingInfo.PaidParking;
                        break;
                    case 5:
                        if (currentDay == DayOfWeek.Saturday || currentDay == DayOfWeek.Sunday)
                        {
                            parkingInfo.PaidParking = false;
                            parkingInfo.TimeInterval = TaxaDTO.Taxa5.WeekendTime;
                            parkingInfo.Price = TaxaDTO.Taxa5.WeekendPrice;
                        }
                        else
                        {
                            parkingInfo.PaidParking = CalculateParkingHours(currentTime, TaxaDTO.Taxa5.WeekDaysTime);
                            parkingInfo.TimeInterval = TaxaDTO.Taxa5.WeekDaysTime;
                            parkingInfo.Price = TaxaDTO.Taxa5.WeekDaysPrice;
                        }
                        break;
                    default:
                        return Results.BadRequest("Unknown parking rate.");
                }

                var parkingExceptions = data.Features[0].Properties.OtherInfo;
                var exceptionMatches = Regex.Matches(
                    parkingExceptions,
                    @"\bServicetid\s+(måndag|tisdag|onsdag|torsdag|fredag|lördag|söndag)\s+(\d{2}:\d{2}-\d{2}:\d{2})",
                    RegexOptions.IgnoreCase
                );

                if (match.Success)
                {
                    string weekday = match.Groups[1].Value;   // tisdag
                    string interval = match.Groups[2].Value;  // 09:00-14:00
                    var isServiceDay = weekday.ToLowerInvariant() switch
                    {
                        "måndag" => currentTime.DayOfWeek == DayOfWeek.Monday,
                        "tisdag" => currentTime.DayOfWeek == DayOfWeek.Tuesday,
                        "onsdag" => currentTime.DayOfWeek == DayOfWeek.Wednesday,
                        "torsdag" => currentTime.DayOfWeek == DayOfWeek.Thursday,
                        "fredag" => currentTime.DayOfWeek == DayOfWeek.Friday,
                        "lördag" => currentTime.DayOfWeek == DayOfWeek.Saturday,
                        "söndag" => currentTime.DayOfWeek == DayOfWeek.Sunday,
                        _ => false
                    };

                    if (isServiceDay)
                    {
                        parkingInfo.CanPark = !CalculateParkingHours(currentTime, interval);
                        parkingInfo.TimeInterval = interval;
                    }
                }






                return Results.Ok(parkingInfo);

                //string weekDaysTime = matches.Count > 0 ? matches[0].Value : string.Empty;
                //string weekendTime = matches.Count > 1 ? matches[1].Value : string.Empty;


                //TODO: Add case if "Alla dagar"
                //var weekDaysMatch = Regex.Match(parkingRate, @"(\d+)\s*kr\/tim\s+vardagar");
                //var otherMatch = Regex.Match(parkingRate, @"(\d+)\s*kr\/tim\s+övrig\s+tid");

                //var weekDaysPrice = weekDaysMatch.Success
                //    ? int.Parse(weekDaysMatch.Groups[1].Value)
                //    : 0;

                //var otherTimePrice = otherMatch.Success
                //    ? int.Parse(otherMatch.Groups[1].Value)
                //    : 0;

                //TODO: Tolka om vi får stå eller inte och skicka med bool


                // Check parking rules based on the day of the week
                //if (currentDay == DayOfWeek.Saturday)
                //{
                //    //paidParking = CalculateParkingHours(currentTime);
                //}
                //else if (currentDay == DayOfWeek.Sunday)
                //{
                //    paidParking = false; //Parking mayhem on sundays
                //}
                //else
                //{
                //    var timeParts = weekDaysTime.Split('-');
                //    if (timeParts.Length == 2 &&
                //        int.TryParse(timeParts[0], out int startHour) &&
                //        int.TryParse(timeParts[1], out int endHour))
                //    {
                //        paidParking = currentTime.Hour >= startHour && currentTime.Hour < endHour;
                //    }
                //    else
                //    {
                //        paidParking = false; // Free parking!
                //    }
                //}

                //TODO Undantag?

                //var parkingDTO = new Parking
                //{
                //    WeekDaysTime = weekDaysTime,
                //    WeekDaysPrice = weekDaysPrice,
                //    WeekendTime = weekendTime,
                //    OtherTimePrice = otherTimePrice,
                //};



                //return Results.Ok(parkingDTO);
            });
        }

        private static bool CalculateParkingHours(DateTime currentTime, string timeInterval)
        {
            bool paidParking;
            var timeParts = timeInterval.Split('-');
            if (timeParts.Length == 2 &&
                int.TryParse(timeParts[0], out int startHour) &&
                int.TryParse(timeParts[1], out int endHour))
            {
                paidParking = currentTime.Hour >= startHour && currentTime.Hour < endHour;
            }
            else
            {
                paidParking = false; // Free parking!
            }

            return paidParking;
        }
    }
}
