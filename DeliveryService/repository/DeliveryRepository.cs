public class DeliveryRepository
{
    public string? ConnectionString { get; set; }     

    public DeliveryRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    internal string ReserveAgent()
    {
        return ConnectionString;
    }

    internal object BookAgent()
    {
        throw new NotImplementedException();
    }
}