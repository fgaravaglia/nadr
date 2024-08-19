using System.Diagnostics.CodeAnalysis;

namespace NADR.Cli.Args
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AddNewRecordCommand
    {
        /// <summary>
        /// if sspecificed, the ID to assign to new record.
        /// </summary>
        public int? Progressive { get; set; }
        /// <summary>
        /// Short Name of new record. it is used to generate base file name for record files
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// full path of Code repository, to store the ADR
        /// </summary>
        public string Repository { get; set; }
        /// <summary>
        /// name of tempalte stored in $(Root)\templates\adr
        /// </summary>
        public string TemplateName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AddNewRecordCommand()
        {
            this.ShortName = "";
            this.Repository = "";
            this.TemplateName = "garavaglia.md";
        }
    }
}