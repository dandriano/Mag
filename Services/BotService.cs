using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phils.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Phils.Services
{
    public class BotService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserRegistrationService _users;
        private readonly IOrderService _orders;
        private readonly ILogger<BotService> _logger;

        public BotService(ITelegramBotClient botClient, IUserRegistrationService userRegistrationService, IOrderService orderService, ILoggerFactory logger)
        {
            _botClient = botClient;
            _users = userRegistrationService;
            _orders = orderService;
            _logger = logger.CreateLogger<BotService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Telegram Bot...");

            var offset = 0; // Initial offset for updates
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            do
            {
                _logger.LogInformation("Gather Updates...");
                var updates = await _botClient.GetUpdatesAsync(offset, cancellationToken: stoppingToken);

                foreach (var update in updates)
                {
                    _logger.LogInformation($"Serving update:\t{update.Type}");
                    offset = update.Id + 1; // Update offset to avoid processing the same update again
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));

            _logger.LogInformation("Stopping Telegram Bot...");
        }
    }
}