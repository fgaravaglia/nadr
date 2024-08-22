using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace NADR.Domain.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RecordRegistryRepositoryFactory : IRecordRegistryRepositoryFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IRecordRegistryRepository InstanceForCsv(string folder, string fileName)
        {
            return new CsvRecordRegistry(folder, fileName);
        }
    }
}