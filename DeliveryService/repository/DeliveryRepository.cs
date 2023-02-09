using MySql.Data;
using MySql.Data.MySqlClient;

public class DeliveryRepository
{
    public string? ConnectionString { get; set; }     

    public DeliveryRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    internal string ReserveAgent()
    {
        using var con = new MySqlConnection(ConnectionString);
        con.Open();

        using var cmd = new MySqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "SELECT * FROM agents";
        MySqlDataReader rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
            Console.WriteLine(rdr[0]+" -- "+rdr[1]);
        }
        rdr.Close();

        return "";
    }

    internal object BookAgent()
    {
        using var con = new MySqlConnection(ConnectionString);
        con.Open();

        using var cmd = new MySqlCommand();
        cmd.Connection = con;

        cmd.CommandText = "SELECT * FROM agents";
        MySqlDataReader rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
            Console.WriteLine(rdr[0]+" -- "+rdr[1]);
        }
        rdr.Close();

        return null;
    }
}