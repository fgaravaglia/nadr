namespace NADR.Cli.Args
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICliArgumentParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        CliCommand ParseCommand(string[] args);
    }

    public class CliArgumentParser : ICliArgumentParser
    {
        public CliCommand ParseCommand(string[] args)
        {
            if (!args.Any())
                throw new InvalidOperationException("[Wrong Usage] No Argument provided");

            if (args[0].ToLower() == "new")
                return ParseArgumentsIntoAddNewRecordCommand(args.ToList());
            else if (args[0].ToLower() == "approve")
                return ParseArgumentsIntoApproveRecordCommand(args.ToList());
            else
                throw new NotImplementedException($"Command {args[0].ToLower()} unknown");
        }

        #region Private Methods

        AddNewRecordCommand ParseArgumentsIntoAddNewRecordCommand(IEnumerable<string> args)
        {
            AddNewRecordCommand cmd = new AddNewRecordCommand();
            foreach (var arg in args.Skip(1))
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

        ApproveRecordCommand ParseArgumentsIntoApproveRecordCommand(IEnumerable<string> args)
        {
            ApproveRecordCommand cmd = new ApproveRecordCommand();
            foreach (var arg in args.Skip(1))
            {
                var option = arg.Split('=').First().Replace("-", "");
                var value = arg.Split('=').Last();
                switch (option.ToLower())
                {
                    case "r":
                        cmd.Repository = value;
                        break;
                    case "p":
                        cmd.Progressive = Int32.Parse(value);
                        break;
                    default:
                        throw new InvalidOperationException($"[Wrong Usage] Option {option} not supported!");
                }
            }
            if (String.IsNullOrEmpty(cmd.Repository))
                throw new InvalidOperationException($"[Wrong Usage] Repository root folder not set (Option: -r)");
            if (cmd.Progressive <= 0)
                throw new InvalidOperationException($"[Wrong Usage]Progressive not set (Option: -p)");
            return cmd;
        }

        #endregion
    }
}