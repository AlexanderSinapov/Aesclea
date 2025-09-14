// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.Server;

namespace Aesclea_Back_End_
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serverManager = new ServerManager();
            
            try
            {
                // Handle Ctrl+C gracefully
                Console.CancelKeyPress += async (sender, e) =>
                {
                    e.Cancel = true;
                    Console.WriteLine("\nShutting down gracefully...");
                    await serverManager.StopAsync();
                    Environment.Exit(0);
                };

                await serverManager.StartAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application error: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}