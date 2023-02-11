public interface IDeliveryRepository
{
    public string ReserveAgent();

    public string BookAgent(string orderId);
}