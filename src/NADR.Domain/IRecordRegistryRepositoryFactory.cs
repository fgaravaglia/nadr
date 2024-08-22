namespace NADR.Domain
{
    /// <summary>
    /// Abstraction to instance a repository
    /// </summary>
    public interface IRecordRegistryRepositoryFactory
    {
        /// <summary>
        /// generate the repository based on CSV
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IRecordRegistryRepository InstanceForCsv(string folder, string fileName);
    }
}