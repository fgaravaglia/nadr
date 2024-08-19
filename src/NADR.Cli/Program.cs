using NADR.Cli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;
using NADR.Domain;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;

// The initial "bootstrap" logger is able to log errors during start-up.
Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console().MinimumLevel.Information()
                    .CreateLogger();

try
{
    // Setup Host
    Log.Logger.Information("Start application Setup");
    var host = IHostBuilderExtensions.CreateDefaultBuilder().Build();
    Log.Logger.Information("End application Setup");

    // get dependencies
    Log.Logger.Information("instancing the target task and run it...");
    var logger = host.Services.GetRequiredService<Microsoft.Extensions.Logging.ILogger>();
    var config = host.Services.GetRequiredService<IConfiguration>();
    var service = host.Services.GetRequiredService<IAdlService>();
    var task = new CliRunnableTask(logger, config, service);
    task.PrintSplashScreen();
    task.ReadEnvironmentVariables();
    // args = new string[]
    // {
    //     @"-r=C:\Projects\GitHub\temp", "-n=authentication"
    // };
    task.SetArgs(args);
    var result = task.Run();
    if (result.Key)
    {
        Log.Logger.Information("Task succesfully run!");
        Environment.Exit(0);
    }
    else
    {
        Log.Logger.Warning("Task failed: " + result.Value);
        Environment.Exit(1);
    }
}
catch (Exception ex)
{
    Log.Logger.Error(ex, "Cli failed: " + ex.Message);
    Environment.Exit(2);
}

