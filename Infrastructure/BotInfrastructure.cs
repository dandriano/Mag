using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mag.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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

            var offset = 0;
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            do
            {
                var updates = await _botClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken);

                foreach (var update in updates)
                {
                    _logger.LogInformation($"Serving update:\t{update.Type}");
                    offset = update.Id + 1;
                    if (update.Type != UpdateType.Message || update.Message == null || string.IsNullOrEmpty(update.Message.Text))
                        continue;


                    var chatId = update.Message.Chat.Id;
                    switch (update.Message.Text)
                    {
                        case "/start":
                            await _botClient.SendTextMessageAsync(chatId, "Welcome! Choose an option:", cancellationToken: cancellationToken);
                            break;
                        case "/menu":
                            var inlineKeyboard = new InlineKeyboardMarkup(
                            [
                                InlineKeyboardButton.WithCallbackData("Option 1", "option1"),
                                InlineKeyboardButton.WithCallbackData("Option 2", "option2"),
                            ]);

                            await _botClient.SendTextMessageAsync(chatId, "Choose an option:", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                            break;
                        default:
                            await _botClient.SendTextMessageAsync(chatId, "Unknown command. Try /menu.", cancellationToken: cancellationToken);
                            break;
                    }
                }
            } while (await timer.WaitForNextTickAsync(cancellationToken));

            _logger.LogInformation("Stopping Telegram Bot...");
        }
    }
}