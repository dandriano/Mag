using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mag.Infrasctructure
{
    public class RestInfrastructure : BackgroundService
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly ILogger<RestInfrastructure> _logger;

        public RestInfrastructure(int port, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RestInfrastructure>();
            _listener.Prefixes.Add($"http://*:{port}/");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting RestInfrastructure...");
            _listener.Start();

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RestInfrastructure...");
            _listener.Stop();
            _listener.Close();
            
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _logger.LogInformation($"Process request:\t{context.Request.UserHostAddress}");
                    _ = ProcessRequestAsync(context, cancellationToken);
                }
                catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Something went  completely wrong!");
                }
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var response = context.Response;
            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "text/html; charset=utf-8";

            _logger.LogInformation($"{request.HttpMethod} {request.Url}");

            var responseString = "<HTML><BODY>Hello World!</BODY></HTML>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using var output = response.OutputStream;
            await output.WriteAsync(buffer, cancellationToken);
            await output.FlushAsync(cancellationToken);
        }
    }
}