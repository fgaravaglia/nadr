using static NADR.Domain.Models.Record;

namespace NADR.Domain
{

    /// <summary>
    /// abstraction for persistence of repcord registry
    /// </summary>
    public interface IRecordRegistryRepository
    {
        /// <summary>
        /// Retrieves the record from registry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IRegistryRecord? GetById(int id);
        /// <summary>
        /// retrieves all records
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRegistryRecord> GetAll();
        /// <summary>
        /// Creates or updates the record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        int Save(IRegistryRecord record);
    }
}