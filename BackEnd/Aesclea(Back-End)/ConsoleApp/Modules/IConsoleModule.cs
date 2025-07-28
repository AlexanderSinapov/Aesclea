namespace Aesclea_Back_End_.ConsoleApp.Modules
{
    public interface IConsoleModule
    {
        Task InitializeAsync();
        Task RunAsync();
        Task<string> GetStatusAsync();
    }
}
