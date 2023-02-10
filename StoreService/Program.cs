        var builder = WebApplication.CreateBuilder(args);
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        builder.Services.AddScoped(provider => config);
        var app = builder.Build();

        StoreRepository repository = new StoreRepository(config);

        app.MapPost("/store/food/reserve", () =>
        {
            return repository.ReserveFood();
        });

        app.MapPost("/store/food/book", () =>
        {
            return repository.BookFood();
        });

        app.Run();
