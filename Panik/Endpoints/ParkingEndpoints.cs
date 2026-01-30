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
                var matches = Regex.Matches(parkingRate, @"\b\d{1,2}-\d{1,2}\b");

                if (matches.Count < 2)
                {
                    return Results.BadRequest("Format is invalid.");
                }

                string weekDaysTime = matches[0].Value;
                string weekendTime = matches[1].Value;

                var weekDaysMatch = Regex.Match(parkingRate, @"(\d+)\s*kr\/tim\s+vardagar");
                var otherMatch = Regex.Match(parkingRate, @"(\d+)\s*kr\/tim\s+övrig\s+tid");

                var weekDaysPrice = weekDaysMatch.Success
                    ? int.Parse(weekDaysMatch.Groups[1].Value)
                    : 0;

                var otherTimePrice = otherMatch.Success
                    ? int.Parse(otherMatch.Groups[1].Value)
                    : 0;

                //TODO: Tolka om vi får stå eller inte och skicka med bool
                var currentTime = DateTime.Now;
                var currentDay = currentTime.DayOfWeek;
                bool paidParking;
                bool canPark;

                // Check parking rules based on the day of the week
                if (currentDay == DayOfWeek.Saturday)
                {
                    var timeParts = weekendTime.Split('-');
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
                }
                else if (currentDay == DayOfWeek.Sunday)
                {
                    paidParking = false; //Parking mayhem on sundays
                }
                else
                {
                    var timeParts = weekDaysTime.Split('-');
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
                }

                //TODO Undantag?

                var parkingDTO = new ParkingDTO
                {
                    WeekDaysTime = weekDaysTime,
                    WeekDaysPrice = weekDaysPrice,
                    WeekendTime = weekendTime,
                    OtherTimePrice = otherTimePrice,
                };



                return Results.Ok(parkingDTO);
            });
        }
    }
}
