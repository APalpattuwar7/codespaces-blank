public interface IDeliveryRepository
{
    public int ReserveAgent();

    public int BookAgent(string orderId);
}