using System.Diagnostics.CodeAnalysis;

namespace NADR.Domain.Models
{
    /// <summary>
    /// Model to map a specific record
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Record : IRegistryRecord
    {
        #region Used Types

        /// <summary>
        /// Status of record
        /// </summary>
        public enum StatusEnum
        {
            /// <summary>
            /// 
            /// </summary>
            Proposed = 0,
            /// <summary>
            /// 
            /// </summary>
            Accepted = 1,
            /// <summary>
            /// 
            /// </summary>
            Deprecated = 2,
            /// <summary>
            /// 
            /// </summary>
            Superseded = 3
        }
        #endregion

        /// <summary>
        /// if sspecificed, the ID to assign to new record.
        /// </summary>
        public int Progressive { get; set; }
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
        public StatusEnum Status { get; set; }
        /// <summary>
        /// /
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// <inheritdoc />>
        /// </summary>
        public List<string> Links { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CurrentVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Record()
        {
            this.ShortName = "";
            this.Repository = "";
            this.TemplateName = "garavaglia.md";
            this.Status = Record.StatusEnum.Proposed;
            this.Links = new List<string>();
            this.CurrentVersion = 1;
        }
    }
}