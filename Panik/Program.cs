
using Panik.Endpoints;
using Scalar.AspNetCore;

namespace Panik
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            ParkingEndpoints.APIKey = builder.Configuration["OpenParkingAPI"] ?? string.Empty;

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            ParkingEndpoints.MapParkingEndpoints(app);

            app.Run();
        }
    }
}
