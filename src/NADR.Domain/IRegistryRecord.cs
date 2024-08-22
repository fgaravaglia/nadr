using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NADR.Domain.Models.Record;

namespace NADR.Domain
{
    /// <summary>
    /// Abstraction to model a specific record to be persisted into registry
    /// </summary>
    public interface IRegistryRecord
    {
        /// <summary>
        /// if sspecificed, the ID to assign to new record.
        /// </summary>
        int Progressive { get; set; }
        /// <summary>
        /// Short Name of new record. it is used to generate base file name for record files
        /// </summary>
        string ShortName { get; set; }
        /// <summary>
        /// /
        /// </summary>
        DateTime CreatedOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        StatusEnum Status { get; set; }
        /// <summary>
        /// links to other records
        /// </summary>
        List<string> Links { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int CurrentVersion { get; set; }
    }
}