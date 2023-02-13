    var builder = WebApplication.CreateBuilder(args);
    var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    builder.Services.AddScoped(provider => config);
    var app = builder.Build();

    int foodId = 1;
    OrderRepository orderRepository = new OrderRepository();

    var t = Task.Run(() => orderRepository.PlaceOrder(foodId));

    // for(int i = 0; i < 10; i++)
    // {
    //     Task.Run(orderRepository.PlaceOrder(foodId));
    // }
    // Parallel.Invoke(
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId),
    //         () => orderRepository.PlaceOrder(foodId));

    app.Run();
