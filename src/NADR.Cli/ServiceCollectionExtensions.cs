using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NADR.Domain;
using Serilog;

namespace NADR.Cli
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Fills dependencies container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddLogging();

            // add here your dependencies
            services.AddTransient<IAdlService, AdlService>();

            return services;
        }
        /// <summary>
        ///  Build provider for depepndencies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceProvider Build(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            return services.BuildServiceProvider();
        }

        #region Private Methods

        static void AddLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory>(x => LoggerFactory.Create(c => c.AddSerilog(Log.Logger)));
            services.AddTransient<Microsoft.Extensions.Logging.ILogger>(x =>
            {
                ILoggerFactory factory = x.GetRequiredService<ILoggerFactory>();
                return factory.CreateLogger("NADR.Cli");
            });
        }
        #endregion
    }
}