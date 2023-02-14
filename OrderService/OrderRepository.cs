using System.Net;
using System.Text;

public class OrderRepository
{
    public async Task<string> PlaceOrder(int foodId)
    {
        using(var httpClient = new HttpClient())
        {
            StringContent content = new StringContent(Convert.ToString(foodId), Encoding.UTF8, "application/json");
            using(var response = await httpClient.PostAsync($"http://localhost:5077/store/food/reserve/{foodId}", content))
            {
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    return "Food not available.";
                }
            }
        }

        using(var httpClient = new HttpClient())
        {
            using(var response = await httpClient.PostAsync($"http://localhost:5166/delivery/agent/reserve", null))
            {
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    return "Agent not available.";
                }
            }
        }

        string orderId = Guid.NewGuid().ToString();

        using(var httpClient = new HttpClient())
        {
            using(var response = await httpClient.PostAsync($"http://localhost:5077/store/food/book/{orderId}", null))
            {
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    return "Food not booked.";
                }
            }
        }

         using(var httpClient = new HttpClient())
        {
            using(var response = await httpClient.PostAsync($"http://localhost:5166/delivery/agent/book/{orderId}", null))
            {
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    return "Agent not booked.";
                }
            }
        }

        return "Order placed successfully";
    }
}