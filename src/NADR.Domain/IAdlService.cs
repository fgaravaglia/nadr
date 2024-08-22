using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NADR.Domain.Models;

namespace NADR.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAdlService
    {
        /// <summary>
        /// Checks existing registry and return next available Id
        /// </summary>
        /// <param name="adrRootPath"></param>
        /// <returns></returns>
        int CalculateNextRecordId(string adrRootPath);
        /// <summary>
        /// Creates a new record
        /// </summary>
        int AddNewRecordToRegistry(string shortName, string repoRootPath, string templateName);
        /// <summary>
        /// approves the record
        /// </summary>
        /// <param name="progressive"></param>
        /// <param name="repoRootPath"></param>
        void ApproveRecord(int progressive, string repoRootPath);
        /// <summary>
        /// deprecates the record
        /// </summary>
        /// <param name="progressive"></param>
        /// <param name="repoRootPath"></param>
        void DeprecateRecord(int progressive, string repoRootPath);
        /// <summary>
        /// Superseds the record
        /// </summary>
        /// <param name="progressive"></param>
        /// <param name="repoRootPath"></param>
        /// <param name="replacingProgressive">
        void SupersedRecord(int progressive, string repoRootPath, int replacingProgressive);
    }
}