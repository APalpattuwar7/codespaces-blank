using System.Net;
using System.Text;

public class OrderRepository
{
    public async Task<string> PlaceOrder(int foodId)
    {
        using(var httpClient = new HttpClient())
        {
            StringContent content = new StringContent(Convert.ToString(foodId), Encoding.UTF8, "application/json");
            using(var response = await httpClient.PostAsync("http://localhost:5166/store/food/reserve", content))
            {
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    return "Food not available.";
                }
            }
        }

        return "";

        // using(var httpClient = new HttpClient())
        // {
        //     using(var response = await httpClient.PostAsync("http://localhost:5166/store/food/reserve"))
        //     {
        //         if(response.StatusCode != HttpStatusCode.OK)
        //         {
        //             return "Food not available.";
        //         }

        //     }
        // }
    }
}