using MySql.Data;
using MySql.Data.MySqlClient;

public class DeliveryRepository : IDeliveryRepository
{
    public string? ConnectionString { get; set; }     

    public DeliveryRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public string ReserveAgent()
    {
        MySqlTransaction myTransaction = null;
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
            int agentId = 0;

            if(!rdr.HasRows)
            {
                rdr.Close();
                myTransaction.Rollback();
                return "No delivery agent available";
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
                return "No delivery agent available";
            }

            reader.Close();
            myTransaction.Commit();
        }
        catch (System.Exception ex)
        {
            myTransaction.Rollback();
            return "No delivery agent available";
        }

        return "Delivery agent reserved.";
    }

    public string BookAgent(string orderId)
    {
        MySqlTransaction myTransaction = null;
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
            int agentId = 0;

            if(!rdr.HasRows)
            {
                rdr.Close();
                myTransaction.Rollback();
                return "No delivery agent available.";
            }

            while (rdr.Read())
            {
                agentId = Convert.ToInt32(rdr[0]);
            }

            rdr.Close();
            cmd.CommandText = $"UPDATE agents set is_reserved = false and order_id = '{orderId}' where id = {agentId}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader.RecordsAffected == 0)
            {
                myTransaction.Rollback();
                return "Unable to book agent.";
            }

            reader.Close();
            myTransaction.Commit();

            return "Agent Booked";
        }
        catch (System.Exception)
        {
            myTransaction.Rollback();
        }

        return "";
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