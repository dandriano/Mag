using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phils.Extensions;
using Phils.Interfaces;
using Phils.Services;
using Serilog;
using Serilog.Extensions.Logging;
using Telegram.Bot;

namespace Phils.Infrasctructure
{
    public static class Host
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                    builder.AddTomlFile("appsettings.toml", optional: false, reloadOnChange: true))
                .ConfigureServices((context, services) =>
                {
                    var botToken = context.Configuration["TelegramBot:Token"];
                    var publicKey = context.Configuration["Admin:PublicKey"];

                    services.AddSingleton<ILoggerFactory, SerilogLoggerFactory>(sp =>
                            new SerilogLoggerFactory(new LoggerConfiguration()
                                                        .MinimumLevel.Debug()
                                                        .WriteTo.Console()
                                                        .WriteTo.File("status.log", rollingInterval: RollingInterval.Day)
                            .CreateLogger()));
                    services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botToken));
                    services.AddScoped<IUserRegistrationService>(_ => new UserRegistrationService(publicKey));

                    services.AddScoped<IOrderService, OrderService>();
                    services.AddHostedService<BotService>();
                });
    }
}