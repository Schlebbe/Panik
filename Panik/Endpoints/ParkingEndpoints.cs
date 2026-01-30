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

                string weekDaysTime = matches[0].Value;
                string weekendTime = matches[1].Value;

                var weekDaysMatch = Regex.Match(parkingRate, @"(\d+)\s*kr\/tim\s+vardagar");
                var ovrigMatch = Regex.Match(parkingRate, @"(\d+)\s*kr\/tim\s+övrig\s+tid");

                var weekDaysPrice = weekDaysMatch.Success
                    ? int.Parse(weekDaysMatch.Groups[1].Value)
                    : 0;

                var otherTimePrice = ovrigMatch.Success
                    ? int.Parse(ovrigMatch.Groups[1].Value)
                    : 0;

                var parkingDTO = new ParkingDTO
                {
                    WeekDaysTime = weekDaysTime,
                    WeekDaysPrice = weekDaysPrice,
                    WeekendTime = weekendTime,
                    OtherDayPrice = otherTimePrice
                };

                return Results.Ok(parkingDTO);
            });
        }
    }
}
