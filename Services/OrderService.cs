using Microsoft.Extensions.Logging;
using Mag.Interfaces;

namespace Mag.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        public OrderService(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<OrderService>();
        }
        public void PlaceOrder(int productId, int quantity)
        {
            // Simulate saving to a database or another storage mechanism (not implemented here)
        }
    }
}