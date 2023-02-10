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

            using var cmd = new MySqlCommand();
            myTransaction = con.BeginTransaction();
    
            cmd.Connection = con;
            cmd.Transaction = myTransaction;
    
            cmd.CommandText = $"SELECT id, food_id, is_reserved, order_id FROM packets WHERE is_reserved is false and food_id = {foodId} and order_id is null LIMIT 1 FOR UPDATE";
            MySqlDataReader rdr = cmd.ExecuteReader();
            int packetId;

            if(rdr == null)
            {
                myTransaction.Rollback();
                return "No food packet available";
            }

            while (rdr.Read())
            {
                Console.WriteLine(rdr[0]);
                packetId = rdr[0];
            }

            cmd.CommandText = $"UPDATE packets set is_reserved = true where id = {packetId}";
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

    public string BookFood(int foodId)
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
            int packetId;

            if(rdr == null)
            {
                myTransaction.Rollback();
                return "No food packet available.";
            }

            while (rdr.Read())
            {
                Console.WriteLine(rdr[0]);
                packetId = rdr[0];
            }

            Guid orderId = Guid.NewGuid();
            cmd.CommandText = $"UPDATE packets set is_reserved = false and order_id = {orderId} where id = {packetId}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader == null)
            {
                myTransaction.Rollback();
                return "";
            }

            rdr.Close();
            myTransaction.Commit();

            return "Food Booked";
        }
        catch (System.Exception)
        {
            myTransaction.Rollback();
        }

        return "";
    }
}