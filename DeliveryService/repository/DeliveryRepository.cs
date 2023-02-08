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
        using var con = new MySqlConnection(ConnectionString);
        con.Open();

        using var cmd = new MySqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "DROP TABLE IF EXISTS cars";
        cmd.ExecuteNonQuery();
    }
}