using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace NADR.Cli
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class IHostBuilderExtensions
    {
        /// <summary>
        /// Creates a new Host
        /// </summary>
        /// <returns></returns>
        public static IHostBuilder CreateDefaultBuilder()
        {
            // get current env
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (String.IsNullOrEmpty(environment))
                environment = "Development";

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile($"appsettings.json", true, true)
                       .AddJsonFile($"appsettings.{environment}.json", true, true)
                       .AddEnvironmentVariables();
                })
                .ConfigureServices(services =>
                {
                    services.AddServices();
                });
        }
    }
}