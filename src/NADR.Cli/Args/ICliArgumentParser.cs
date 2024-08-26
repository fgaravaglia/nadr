using System.Text;

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
        CliCommand? ParseCommand(string[] args);
    }

    public class CliArgumentParser : ICliArgumentParser
    {
        public CliCommand? ParseCommand(string[] args)
        {
            if (!args.Any())
                throw new InvalidOperationException("[Wrong Usage] No Argument provided. For additional info use 'NADRS.exe help'");

            if (args[0].ToLower() == "new")
                return ParseArgumentsIntoAddNewRecordCommand(args.ToList());
            else if (args[0].ToLower() == "approve")
                return ParseArgumentsIntoApproveRecordCommand(args.ToList());
            else if (args[0].ToLower() == "deprecate")
                return ParseArgumentsIntoDeprecateRecordCommand(args.ToList());
            else if (args[0].ToLower() == "supersed")
                return ParseArgumentsIntoSupersedRecordCommand(args.ToList());
            else if (args[0].ToLower() == "help")
            {
                StringBuilder usage = new StringBuilder();
                usage.AppendLine();
                usage.AppendLine("***************************************************************");
                usage.AppendLine("\tUsage of CLI");
                usage.AppendLine("***************************************************************");
                usage.AppendLine("new\t\tCreates a new record");
                usage.AppendLine("\t-r\tSpecify the root folder of Git Repository");
                usage.AppendLine("\t-n\tSpecify the short name to generate proper .md file and folders");
                usage.AppendLine();
                usage.AppendLine("approve\t\tApproves an existing record");
                usage.AppendLine("\t-r\tSpecify the root folder of Git Repository");
                usage.AppendLine("\t-p\tSpecify the target record id");
                usage.AppendLine();
                usage.AppendLine("decrecate\tDeprecates an existing record");
                usage.AppendLine("\t-r\tSpecify the root folder of Git Repository");
                usage.AppendLine("\t-p\tSpecify the target record id");
                usage.AppendLine();
                usage.AppendLine("supersed\tSuperseds an existing record");
                usage.AppendLine("\t-r\tSpecify the root folder of Git Repository");
                usage.AppendLine("\t-p\tSpecify the target record id");
                usage.AppendLine("\t-n\tSpecify the id of record that replace the target ones");

                Console.WriteLine(usage.ToString());
                return null;
            }
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

        DeprecateRecordCommand ParseArgumentsIntoDeprecateRecordCommand(IEnumerable<string> args)
        {
            DeprecateRecordCommand cmd = new DeprecateRecordCommand();
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
                throw new InvalidOperationException($"[Wrong Usage] Progressive not set (Option: -p)");
            return cmd;
        }

        SupersedRecordCommand ParseArgumentsIntoSupersedRecordCommand(IEnumerable<string> args)
        {
            SupersedRecordCommand cmd = new SupersedRecordCommand();
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
                    case "n":
                        cmd.ReplacingId = Int32.Parse(value);
                        break;
                    default:
                        throw new InvalidOperationException($"[Wrong Usage] Option {option} not supported!");
                }
            }
            if (String.IsNullOrEmpty(cmd.Repository))
                throw new InvalidOperationException($"[Wrong Usage] Repository root folder not set (Option: -r)");
            if (cmd.Progressive <= 0)
                throw new InvalidOperationException($"[Wrong Usage] Progressive not set (Option: -p)");
            if (cmd.ReplacingId <= 0)
                throw new InvalidOperationException($"[Wrong Usage] Replacing Id not set (Option: -n)");
            return cmd;
        }
        #endregion
    }
}