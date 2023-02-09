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
        MySqlTransaction myTransaction = null;
        try
        {
            using var con = new MySqlConnection(ConnectionString);
            con.Open();

            using var cmd = new MySqlCommand();
            myTransaction = con.BeginTransaction();
    
            cmd.Connection = con;
            cmd.Transaction = myTransaction;
    
            cmd.CommandText = "SELECT id, is_reserved, order_id FROM agents WHERE is_reserved is false and order_id is null LIMIT 1 FOR UPDATE";
            MySqlDataReader rdr = cmd.ExecuteReader();
            int agentId;

            if(rdr == null)
            {
                myTransaction.Rollback();
                return "No delivery agent available";
            }

            while (rdr.Read())
            {
                Console.WriteLine(rdr[0]);
                agentId = rdr[0];
            }

            cmd.CommandText = $"UPDATE agents set is_reserved = true where id = {agentId}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader == null)
            {
                myTransaction.Rollback();
                return "";
            }

            rdr.Close();
            myTransaction.Commit();
        }
        catch (System.Exception)
        {
            myTransaction.Rollback();
        }

        return "";
    }

    internal string BookAgent()
    {
        MySqlTransaction myTransaction = null;
        try
        {
            using var con = new MySqlConnection(ConnectionString);
            con.Open();

            using var cmd = new MySqlCommand();
            myTransaction = con.BeginTransaction();
    
            cmd.Connection = con;
            cmd.Transaction = myTransaction;
    
            cmd.CommandText = "SELECT id, is_reserved, order_id FROM agents WHERE is_reserved is true and order_id is null LIMIT 1 FOR UPDATE";
            MySqlDataReader rdr = cmd.ExecuteReader();
            int agentId;

            if(rdr == null)
            {
                myTransaction.Rollback();
                return "No delivery agent available.";
            }

            while (rdr.Read())
            {
                Console.WriteLine(rdr[0]);
                agentId = rdr[0];
            }

            Guid orderId = Guid.NewGuid();
            cmd.CommandText = $"UPDATE agents set is_reserved = false and order_id = {orderId} where id = {agentId}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader == null)
            {
                myTransaction.Rollback();
                return "";
            }

            rdr.Close();
            myTransaction.Commit();

            return "Agent Booked";
        }
        catch (System.Exception)
        {
            myTransaction.Rollback();
        }

        return "";
    }
}