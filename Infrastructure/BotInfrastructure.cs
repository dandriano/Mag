using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mag.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace Mag.Infrasctructure
{
    public class BotInfrastructure : BackgroundService, IBotInfrastructure
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<BotInfrastructure> _logger;
        public event EventHandler<string> LotReceived;
        public event EventHandler<string> LotCommentReceived;

        public BotInfrastructure(ITelegramBotClient botClient, ILoggerFactory logger)
        {
            _botClient = botClient;
            _logger = logger.CreateLogger<BotInfrastructure>();

            LotReceived += (o, s) => _logger.LogInformation($"Serving lot text:\ts");
            LotCommentReceived += (o, s) => _logger.LogInformation($"Serving lot comment text:\ts");
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
                    offset = update.Id + 1;
                    if (update.Type != UpdateType.Message || update.Message?.Text != string.Empty)
                        continue;

                    if (IsBotCommand(update.Message.Text))
                    {
                        await ProcessCommand(update.Message, cancellationToken);
                    }
                    else if (update.Message.ReplyToMessage != null)
                    {
                        // if lot comment
                        LotCommentReceived?.Invoke(this, update.Message.Text);
                    }
                    else
                    {
                        // if lot
                        LotReceived?.Invoke(this, update.Message.Text);
                    }
                }
            } while (await timer.WaitForNextTickAsync(cancellationToken));

            _logger.LogInformation("Stopping Telegram Bot...");
        }

        private bool IsBotCommand(string messageText) => messageText.StartsWith('/');

        private async Task ProcessCommand(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            switch (message.Text?.ToLower())
            {
                case "/start":
                    await _botClient.SendTextMessageAsync(chatId, "Welcome! I'm here to assist you.", cancellationToken: cancellationToken);
                    break;
                case "/help":
                    await _botClient.SendTextMessageAsync(chatId, "Here are some commands you can use...", cancellationToken: cancellationToken);
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
    }
}