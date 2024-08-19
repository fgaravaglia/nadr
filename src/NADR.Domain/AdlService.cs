using Microsoft.Extensions.Logging;

namespace NADR.Domain;

/// <summary>
/// Service to expose capabilitieis to interact with ADL
/// </summary>
public class AdlService : IAdlService
{
    #region Fields
    readonly ILogger _Logger;
    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AdlService(ILogger logger)
    {
        this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /// <summary>
    /// Checks existing registry and return next available Id
    /// </summary>
    /// <param name="adrRootPath"></param>
    /// <returns></returns>
    public int CalculateNextRecordId(string adrRootPath)
    {
        using (this._Logger.BeginScope(new Dictionary<string, string>()
        {
            ["ClassName"] = this.GetType()?.FullName ?? "",
            ["MethodName"] = nameof(CalculateNextRecordId)
        }))
        {
            ArgumentNullException.ThrowIfNullOrEmpty(adrRootPath);

            // calculating the progressive
            var adrRootDirectory = new DirectoryInfo(adrRootPath);
            var foldersNames = adrRootDirectory.GetDirectories().Select(x => x.Name).OrderBy(x => x);
            if (!foldersNames.Any())
                return 1;

            // take last and extract the id
            var laststringProgressive = foldersNames.Last().Split('-').FirstOrDefault() ?? "0000";
            int progressive;
            if (Int32.TryParse(laststringProgressive, out progressive))
                return progressive + 1;
            else
                throw new InvalidOperationException("Unable to calculate new record progressive id");
        }
    }
}
