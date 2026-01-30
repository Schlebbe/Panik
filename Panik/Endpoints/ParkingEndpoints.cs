namespace Panik.Endpoints
{
    public class ParkingEndpoints
    {
        public static string APIKey { get; set; } = string.Empty;
        public static void MapParkingEndpoints(WebApplication app)
        {
            app.MapGet("/parking", (HttpContext httpContext, string lat, string lng) =>
            {

                return Results.Ok("Parking endpoint is working!");
            });
        }
    }
}
