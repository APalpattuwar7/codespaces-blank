        var builder = WebApplication.CreateBuilder(args);
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        builder.Services.AddScoped(provider => config);
        var app = builder.Build();

        StoreRepository repository = new StoreRepository(config);

        app.MapPost("/store/food/reserve/{foodId}", (int foodId) =>
        {
            return repository.ReserveFood(foodId);
        });

        app.MapPost("/store/food/book/{orderId}", (string orderId) =>
        {
            return repository.BookFood(orderId);
        });

        app.Run();
