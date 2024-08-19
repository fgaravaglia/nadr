using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}