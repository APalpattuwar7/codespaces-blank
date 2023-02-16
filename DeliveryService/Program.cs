using EvolveDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        builder.Services.AddScoped(provider => config);
        var app = builder.Build();
        MigrateDatabase(config);

        DeliveryRepository repository = new DeliveryRepository(config);

        app.MapPost("/delivery/agent/reserve", IResult () =>
        {
            int response = repository.ReserveAgent();
            return response != -1 ? TypedResults.Ok<int>(response) : TypedResults.StatusCode(500);
        });

        app.MapPost("/delivery/agent/book/{orderId}", IResult (string orderId) =>
        {
            int response = repository.BookAgent(orderId);
            return response != -1 ? TypedResults.Ok<int>(response) : TypedResults.StatusCode(500);
        });

        app.Run();
    }

    private static void MigrateDatabase(IConfiguration config)
    {
            // exclude db/datasets from production and staging environments
            // string location = EnvironmentName == Environments.Production || EnvironmentName == Environments.Staging
            //     ? "db/migrations"
            //     : "db";

            string location = "db/migrations";

            try
            {
                var cnx = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString("DefaultConnection"));
                var evolve = new Evolve(cnx)
                {
                    Locations = new[] { location },
                    IsEraseDisabled = true
                };

                evolve.Migrate();
            }
            catch (Exception ex)
            {
                //Log.Error("Database migration failed.", ex);
                throw;
            }
    }
}