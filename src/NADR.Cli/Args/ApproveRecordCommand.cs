using System.Diagnostics.CodeAnalysis;

namespace NADR.Cli.Args
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApproveRecordCommand : CliCommand
    {
        /// <summary>
        /// if sspecificed, the ID to assign to new record.
        /// </summary>
        public int Progressive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ApproveRecordCommand()
        {

        }
    }
}