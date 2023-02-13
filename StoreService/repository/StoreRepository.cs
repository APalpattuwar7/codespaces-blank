using MySql.Data;
using MySql.Data.MySqlClient;

public class StoreRepository
{
    public string? ConnectionString { get; set; }     

    public StoreRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public string ReserveFood(int foodId)
    {
        MySqlTransaction myTransaction = null;
        try
        {
            using var con = new MySqlConnection(ConnectionString);
            con.Open();
            MySqlCommand cmd = CreateMySqlCommand(@$"SELECT id, food_id, is_reserved, order_id 
                                                  FROM packets 
                                                  WHERE is_reserved is false and food_id = {foodId} and order_id is null 
                                                  LIMIT 1 
                                                  FOR UPDATE", ref myTransaction, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            int packetId = 0;

            if(!rdr.HasRows)
            {
                rdr.Close();
                myTransaction.Rollback();
                return "No food packet available";
            }

            while (rdr.Read())
            {
                packetId = Convert.ToInt32(rdr[0]);
            }

            rdr.Close();

            cmd.CommandText = $"UPDATE packets set is_reserved = true where id = {packetId}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader.RecordsAffected == 0)
            {
                myTransaction.Rollback();
                return "No food packet available";
            }

            reader.Close();
            myTransaction.Commit();
        }
        catch (System.Exception ex)
        {
            myTransaction.Rollback();
            return "No food packet available";
        }

        return "Food packet reserved.";
    }

    public string BookFood(string orderId)
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
    
            cmd.CommandText = "SELECT id, is_reserved, order_id FROM packets WHERE is_reserved is true and order_id is null LIMIT 1 FOR UPDATE";
            MySqlDataReader rdr = cmd.ExecuteReader();
            int packetId = 0;

            if(rdr == null)
            {
                rdr.Close();
                myTransaction.Rollback();
                return "No food packet available.";
            }

            while (rdr.Read())
            {
                packetId = Convert.ToInt32(rdr[0]);
            }
            rdr.Close();

            cmd.CommandText = $"UPDATE packets set is_reserved = false, order_id = '{orderId}' where id = {packetId}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader.RecordsAffected == 0)
            {
                reader.Close();
                myTransaction.Rollback();
                return "";
            }

            reader.Close();
            myTransaction.Commit();

            return "Food packet Booked";
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