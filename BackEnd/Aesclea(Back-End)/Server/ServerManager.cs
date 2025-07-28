using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;
using Aesclea_Back_End_.ConsoleApp;

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

            global::System.Console.WriteLine("Aesclea Medical Management System");
            global::System.Console.WriteLine("================================");
            global::System.Console.WriteLine();
            global::System.Console.WriteLine("Select mode:");
            global::System.Console.WriteLine("1. Console Mode (AI Training/Testing)");
            global::System.Console.WriteLine("2. Web Server Mode (API Server)");
            global::System.Console.WriteLine("3. Both Modes (Console + Web Server)");
            global::System.Console.Write("Enter your choice (1-3): ");

            var choice = global::System.Console.ReadLine();

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
                    global::System.Console.WriteLine("Invalid choice. Starting console mode by default.");
                    await RunConsoleMode();
                    break;
            }
        }

        private async Task RunConsoleMode()
        {
            global::System.Console.WriteLine("Starting Console Mode for AI Training/Testing...");
            global::System.Console.WriteLine("Press Ctrl+C to exit.");
            global::System.Console.WriteLine();

            // Run the organized console application
            var consoleApp = new OrganizedConsoleApplication();
            await consoleApp.RunAsync();
        }

        private async Task RunWebServerMode(string[] args)
        {
            global::System.Console.WriteLine("Starting Web Server Mode...");
            global::System.Console.WriteLine("API will be available at: https://localhost:7000");
            global::System.Console.WriteLine("Swagger UI will be available at: https://localhost:7000/swagger");
            global::System.Console.WriteLine("Press Ctrl+C to exit.");
            global::System.Console.WriteLine();

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
            global::System.Console.WriteLine("Starting Both Console and Web Server Modes...");
            global::System.Console.WriteLine("API will be available at: https://localhost:7000");
            global::System.Console.WriteLine("Console commands available below.");
            global::System.Console.WriteLine("Press Ctrl+C to exit both modes.");
            global::System.Console.WriteLine();

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
                var consoleTask = Task.Run(async () =>
                {
                    var consoleApp = new OrganizedConsoleApplication();
                    await consoleApp.RunAsync();
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
