using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NADR.Cli.Args;
using NADR.Domain;

namespace NADR.Cli
{
    /// <summary>
    /// class to implement the task of cli
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CliRunnableTask
    {
        #region Fields
        protected readonly ILogger _Logger;
        protected readonly IConfiguration _Config;
        readonly IAdlService _Service;
        protected List<string> _Arguments;
        #endregion

        public CliRunnableTask(ILogger logger, IConfiguration config, IAdlService service)
        {
            this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._Config = config ?? throw new ArgumentNullException(nameof(config));
            this._Service = service ?? throw new ArgumentNullException(nameof(service));
            this._Arguments = new List<string>();
        }

        /// <summary>
        /// Reads parameters from Env Variables
        /// </summary>
        public void ReadEnvironmentVariables()
        {
            this._Logger.LogInformation("{0}: {1}", nameof(CliRunnableTask), nameof(ReadEnvironmentVariables));
            // todo
        }
        /// <summary>
        /// Sets the arguments of the task
        /// </summary>
        /// <param name="args"></param>
        public void SetArgs(string[] args)
        {
            this._Logger.LogInformation("{0}: {1} Start", this.GetType().Name, nameof(SetArgs));
            this._Arguments.Clear();
            if (args != null && args.Any())
                this._Arguments.AddRange(args);
            this._Logger.LogInformation("{0}: {1} End", this.GetType().Name, nameof(SetArgs));
        }
        /// <summary>
        /// 
        /// </summary>
        public void PrintSplashScreen()
        {
            StringBuilder message = new StringBuilder();

            var assemblyVersion = "0.0.0";
            try
            {
                assemblyVersion = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0";
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex, "Unable to retreive assembly version of NADR.Cli");
            }

            message.AppendLine("");
            message.AppendLine("***************************************************************");
            message.AppendLine("\tWelcome to Net Architecture Decision Records Cli (NADR.Cli) - v" + assemblyVersion);
            message.AppendLine("\tRelease date: September 2024");
            message.AppendLine("***************************************************************");

            this._Logger.LogInformation(message.ToString());
        }
        /// <summary>
        /// Runs the task
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Run()
        {
            string methodName = this.GetType().Name + "." + nameof(Run);
            this._Logger.LogInformation("{0}: {1} Start", nameof(CliRunnableTask), methodName);
            try
            {
                // Parsing Arguments
                var cmd = ParseArgumentsIntoCommand();

                var id = this._Service.AddNewRecordToRegistry(cmd.ShortName, cmd.Repository, cmd.TemplateName);
                this._Logger.LogInformation("Record {RecordId} succesfully created", id);
                return new KeyValuePair<bool, string>(true, "");
            }
            catch (Exception ex)
            {

                this._Logger.LogError(ex, "{0}: {1} Failed! {2}", this.GetType().Name, methodName, ex.Message);
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
            finally
            {
                this._Logger.LogInformation("{0}: {1} End", nameof(CliRunnableTask), methodName);
            }
        }

        #region Private Methods

        AddNewRecordCommand ParseArgumentsIntoCommand()
        {
            if (!this._Arguments.Any())
                throw new InvalidOperationException("[Wrong Usage] No Argument provided");
            AddNewRecordCommand cmd = new AddNewRecordCommand();
            foreach (var arg in this._Arguments)
            {
                var option = arg.Split('=').First().Replace("-", "");
                var value = arg.Split('=').Last();
                switch (option.ToLower())
                {
                    case "r":
                        cmd.Repository = value;
                        break;
                    case "n":
                        cmd.ShortName = value;
                        break;
                    default:
                        throw new InvalidOperationException($"[Wrong Usage] Option {option} not supported!");
                }
            }
            if (String.IsNullOrEmpty(cmd.Repository))
                throw new InvalidOperationException($"[Wrong Usage] Repository root folder not set (Option: -r)");
            if (String.IsNullOrEmpty(cmd.ShortName))
                throw new InvalidOperationException($"[Wrong Usage] Short Name not set (Option: -n)");
            if (String.IsNullOrEmpty(cmd.TemplateName))
                throw new InvalidOperationException($"[Wrong Usage] Tempalte Name not set (Option: -t)");
            return cmd;
        }
        #endregion
    }
}