using System.Globalization;
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
            this._Logger.LogInformation(methodName + " Start");
            try
            {
                // Parsing Arguments
                var cmd = ParseArgumentsIntoCommand();

                // create folder tree
                var adrRootPath = Path.Combine(cmd.Repository, "docs", "adr");
                if (!Directory.Exists(adrRootPath))
                {
                    this._Logger.LogInformation("Creating target folder for ADR: /docs/adr");
                    Directory.CreateDirectory(adrRootPath);
                }

                // calculating the progressive
                var adrRootDirectory = new DirectoryInfo(adrRootPath);
                cmd.Progressive = _Service.CalculateNextRecordId(adrRootPath);

                // Creating new item from template
                CopyFromTemplate(cmd, adrRootPath);

                // update registry
                AddNewRecordToRegistry(cmd, adrRootPath);

                return new KeyValuePair<bool, string>(true, "");
            }
            catch (Exception ex)
            {
                this._Logger.LogError(ex, "Unexpected error during running task {0}: {1}", this.GetType().Name, ex.Message);
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
            finally
            {
                this._Logger.LogInformation(methodName + " End");
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

        int CalculateNextRecordId(string adrRootPath)
        {
            // calculating the progressive
            var adrRootDirectory = new DirectoryInfo(adrRootPath);
            var foldersNames = adrRootDirectory.GetDirectories().Select(x => x.Name).OrderBy(x => x);
            if (!foldersNames.Any())
                return 1;

            // take last and extract the id
            var laststringProgressive = foldersNames.Last().Split('-').FirstOrDefault() ?? "0000";
            int progressive;
            if (Int32.TryParse(laststringProgressive, out progressive))
                return progressive + 1;
            else
                throw new InvalidOperationException("Unable to calculate new record progressive id");
        }

        void CopyFromTemplate(AddNewRecordCommand cmd, string adrRootPath)
        {
            // preparing folder
            string folderName = cmd.Progressive?.ToString("0000") + "-" + cmd.ShortName;
            this._Logger.LogInformation("Creating target folder for record: {targetRecordFolder}", folderName);
            Directory.CreateDirectory(Path.Combine(adrRootPath, folderName));

            // copying template
            this._Logger.LogInformation("Copying template {Template} to create new record", folderName);
            File.Copy(Path.Combine(cmd.Repository, "templates", "adr", cmd.TemplateName),
                        Path.Combine(adrRootPath, folderName, folderName + ".md"));
            Directory.CreateDirectory(Path.Combine(adrRootPath, folderName, "img"));

        }

        void AddNewRecordToRegistry(AddNewRecordCommand cmd, string adrRootPath)
        {
            StringBuilder newRow = new StringBuilder();
            //Progressive; short name; Creation Date;
            newRow.Append(cmd.Progressive?.ToString("0000") ?? "").Append(";");
            newRow.Append(cmd.ShortName).Append(";");
            newRow.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            File.AppendAllLines(Path.Combine(adrRootPath, "adr.csv"), new string[] { newRow.ToString() });
        }
        #endregion
    }
}