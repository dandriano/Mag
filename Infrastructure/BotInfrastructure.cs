using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mag.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Mag.Infrasctructure
{
    public class BotInfrastructure : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserRegistrationService _users;
        private readonly IOrderService _orders;
        private readonly ILogger<BotInfrastructure> _logger;

        public BotInfrastructure(ITelegramBotClient botClient, IUserRegistrationService userRegistrationService, IOrderService orderService, ILoggerFactory logger)
        {
            _botClient = botClient;
            _users = userRegistrationService;
            _orders = orderService;
            _logger = logger.CreateLogger<BotInfrastructure>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Telegram Bot...");

            var offset = 0; // Initial offset for updates
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            do
            {
                _logger.LogInformation("Gather Updates...");
                var updates = await _botClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken);

                foreach (var update in updates)
                {
                    _logger.LogInformation($"Serving update:\t{update.Type}");
                    offset = update.Id + 1; // Update offset to avoid processing the same update again
                }
            } while (await timer.WaitForNextTickAsync(cancellationToken));

            _logger.LogInformation("Stopping Telegram Bot...");
        }
    }
}