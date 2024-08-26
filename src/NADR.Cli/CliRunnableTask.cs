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
        readonly ICliArgumentParser _ArgumentParser;
        readonly IAdlService _Service;
        protected List<string> _Arguments;
        #endregion

        public CliRunnableTask(ILogger logger, IConfiguration config, ICliArgumentParser argParser, IAdlService service)
        {
            this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._Config = config ?? throw new ArgumentNullException(nameof(config));
            this._ArgumentParser = argParser ?? throw new ArgumentNullException(nameof(argParser));
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
                var cmd = this._ArgumentParser.ParseCommand(this._Arguments.ToArray());
                if (cmd == null)
                    return new KeyValuePair<bool, string>(true, "");

                switch (cmd.GetType().Name)
                {
                    case nameof(AddNewRecordCommand):
                        AddNewRecordCommand addCmd = (AddNewRecordCommand)cmd;
                        var id = this._Service.AddNewRecordToRegistry(addCmd.ShortName, addCmd.Repository, addCmd.TemplateName);
                        this._Logger.LogInformation("Record {RecordId} succesfully created", id);
                        break;
                    case nameof(ApproveRecordCommand):
                        ApproveRecordCommand approveCmd = (ApproveRecordCommand)cmd;
                        this._Service.ApproveRecord(approveCmd.Progressive, approveCmd.Repository);
                        this._Logger.LogInformation("Record {RecordId} succesfully approved", approveCmd.Progressive);
                        break;
                    case nameof(DeprecateRecordCommand):
                        DeprecateRecordCommand deprecateCmd = (DeprecateRecordCommand)cmd;
                        this._Service.DeprecateRecord(deprecateCmd.Progressive, deprecateCmd.Repository);
                        this._Logger.LogInformation("Record {RecordId} succesfully deprecated", deprecateCmd.Progressive);
                        break;
                    case nameof(SupersedRecordCommand):
                        SupersedRecordCommand supersedCmd = (SupersedRecordCommand)cmd;
                        this._Service.SupersedRecord(supersedCmd.Progressive, supersedCmd.Repository, supersedCmd.ReplacingId);
                        this._Logger.LogInformation("Record {RecordId} succesfully supeseded by {newRecordId}", supersedCmd.Progressive, supersedCmd.ReplacingId);
                        break;
                    default:
                        throw new NotImplementedException($"Command {cmd.GetType().Name} unknown");
                }

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

    }
}