using MySql.Data;
using MySql.Data.MySqlClient;

public class StoreRepository
{
    public string? ConnectionString { get; set; }     

    public StoreRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public int ReserveFood(int foodId)
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
                Console.WriteLine("No food packet available");
                return -1;
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
                Console.WriteLine("No food packet available");
                return -1;
            }

            reader.Close();
            myTransaction.Commit();
        }
        catch (System.Exception ex)
        {
            myTransaction.Rollback();
            Console.WriteLine("No food packet available");
            return -1;
        }

        Console.WriteLine("Food packet reserved.");
        return foodId;
    }

    public int BookFood(string orderId)
    {
        MySqlTransaction myTransaction = null;
        int packetId = 0;
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
        
            if(rdr == null)
            {
                rdr.Close();
                myTransaction.Rollback();
                Console.WriteLine("No food packet available");
                return -1;
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
                Console.WriteLine("No food packet available");
                return -1;
            }

            reader.Close();
            myTransaction.Commit();    
        }
        catch (System.Exception)
        {
            myTransaction.Rollback();
            return -1;
        }

        Console.WriteLine("Food packet Booked");
        return packetId;
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