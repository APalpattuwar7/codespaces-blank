    var builder = WebApplication.CreateBuilder(args);
    var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    builder.Services.AddScoped(provider => config);
    var app = builder.Build();

    StoreRepository storeRepository = new StoreRepository(config);
    DeliveryRepository deliveryRepository = new DeliveryRepository(config);

    app.Run();
