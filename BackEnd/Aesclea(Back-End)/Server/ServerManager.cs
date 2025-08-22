using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;

namespace Aesclea_Back_End_.Server
{
    public class ServerManager
    {
        private readonly WebServer _webServer;
        private readonly ILogger<ServerManager> _logger;
        private CancellationTokenSource? _cancellationTokenSource;

        public ServerManager()
        {
            _webServer = new WebServer();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ServerManager>();
        }

        public async Task StartAsync(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine("Aesclea Medical Management System");
            Console.WriteLine("================================");
            Console.WriteLine();
            Console.WriteLine("Select mode:");
            Console.WriteLine("1. Console Mode (AI Training/Testing)");
            Console.WriteLine("2. Web Server Mode (API Server)");
            Console.WriteLine("3. Both Modes (Console + Web Server)");
            Console.Write("Enter your choice (1-3): ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await RunConsoleMode();
                    break;
                case "2":
                    await RunWebServerMode(args);
                    break;
                case "3":
                    await RunBothModes(args);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Starting console mode by default.");
                    await RunConsoleMode();
                    break;
            }
        }

        private async Task RunConsoleMode()
        {
            Console.WriteLine("Starting Console Mode for AI Training/Testing...");
            Console.WriteLine("Press Ctrl+C to exit.");
            Console.WriteLine();

            // Run the original AI console application
            await Task.Run(() => ConsoleApplication.Run(), _cancellationTokenSource.Token);
        }

        private async Task RunWebServerMode(string[] args)
        {
            Console.WriteLine("Starting Web Server Mode...");
            Console.WriteLine("API will be available at: https://localhost:7000");
            Console.WriteLine("Swagger UI will be available at: https://localhost:7000/swagger");
            Console.WriteLine("Press Ctrl+C to exit.");
            Console.WriteLine();

            try
            {
                await _webServer.StartAsync(args);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Web server shutdown requested");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in web server mode");
            }
        }

        private async Task RunBothModes(string[] args)
        {
            Console.WriteLine("Starting Both Console and Web Server Modes...");
            Console.WriteLine("API will be available at: https://localhost:7000");
            Console.WriteLine("Console commands available below.");
            Console.WriteLine("Press Ctrl+C to exit both modes.");
            Console.WriteLine();

            try
            {
                // Start web server in background
                var webServerTask = Task.Run(async () =>
                {
                    try
                    {
                        await _webServer.StartAsync(args);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Web server shutdown requested");
                    }
                }, _cancellationTokenSource.Token);

                // Start console application in foreground
                var consoleTask = Task.Run(() =>
                {
                    var consoleApp = new ConsoleApplication();
                    ConsoleApplication.Run();
                }, _cancellationTokenSource.Token);

                // Wait for either to complete
                await Task.WhenAny(webServerTask, consoleTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in combined mode");
            }
        }

        public async Task StopAsync()
        {
            _cancellationTokenSource?.Cancel();
            await _webServer.StopAsync();
        }
    }
}