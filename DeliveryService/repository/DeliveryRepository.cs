using MySql.Data;
using MySql.Data.MySqlClient;

public class DeliveryRepository : IDeliveryRepository
{
    public string? ConnectionString { get; set; }     

    public DeliveryRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public int ReserveAgent()
    {
        MySqlTransaction myTransaction = null;
        int agentId = 0;
        try
        {
            using var con = new MySqlConnection(ConnectionString);
            con.Open();
            MySqlCommand cmd = CreateMySqlCommand(@"SELECT id, is_reserved, order_id 
                                                  FROM agents 
                                                  WHERE is_reserved is false and order_id is null 
                                                  LIMIT 1 
                                                  FOR UPDATE", ref myTransaction, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            
            if(!rdr.HasRows)
            {
                rdr.Close();
                myTransaction.Rollback();
                Console.WriteLine("No delivery agent available");
                return -1;
            }

            while (rdr.Read())
            {
                agentId = Convert.ToInt32(rdr[0]);
            }

            rdr.Close();

            cmd.CommandText = $"UPDATE agents set is_reserved = true where id = {agentId}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader.RecordsAffected == 0)
            {
                myTransaction.Rollback();
                Console.WriteLine("No delivery agent available");
                return -1;
            }

            reader.Close();
            myTransaction.Commit();
        }
        catch (System.Exception ex)
        {
            myTransaction.Rollback();
            Console.WriteLine("No delivery agent available");
            return -1;
        }

        Console.WriteLine("Delivery agent reserved");
        return agentId;
    }

    public int BookAgent(string orderId)
    {
        MySqlTransaction myTransaction = null;
        int agentId = 0;
        try
        {
            using var con = new MySqlConnection(ConnectionString);
            con.Open();
            MySqlCommand cmd = CreateMySqlCommand(@"SELECT id, is_reserved, order_id 
                                                  FROM agents 
                                                  WHERE is_reserved is true and order_id is null 
                                                  LIMIT 1 
                                                  FOR UPDATE", ref myTransaction, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            
            if(!rdr.HasRows)
            {
                rdr.Close();
                myTransaction.Rollback();
                Console.WriteLine("Unable to book agent");
                return -1;
            }

            while (rdr.Read())
            {
                agentId = Convert.ToInt32(rdr[0]);
            }

            rdr.Close();
            cmd.CommandText = $"UPDATE agents set is_reserved = false, order_id = '{orderId}' where id = {agentId}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader.RecordsAffected == 0)
            {
                reader.Close();
                myTransaction.Rollback();
                Console.WriteLine("Unable to book agent");
                return -1;
            }

            reader.Close();
            myTransaction.Commit();
        }
        catch (System.Exception)
        {
            //reader.Close();
            myTransaction.Rollback();
            Console.WriteLine("Unable to book agent");
            return -1;
        }

        Console.WriteLine("Agent Booked");
        return agentId;
    }

    private MySqlCommand CreateMySqlCommand(string query, ref MySqlTransaction myTransaction, MySqlConnection con)
    {
        using var cmd = new MySqlCommand();
        myTransaction = con.BeginTransaction();

        cmd.Connection = con;
        cmd.Transaction = myTransaction;

        cmd.CommandText = query;
        return cmd;
    }
}