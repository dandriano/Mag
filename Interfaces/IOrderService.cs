namespace Mag.Interfaces
{
    public interface IOrderService
    {
        void PlaceOrder(int productId, int quantity);
    }
}