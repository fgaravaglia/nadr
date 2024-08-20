using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using NADR.Domain.Models;

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

    #region Private Methods

    Dictionary<string, string> CreateScope(string methodName)
    {
        return new Dictionary<string, string>()
        {
            ["ClassName"] = this.GetType()?.FullName ?? "",
            ["MethodName"] = methodName
        };

    }

    /// <summary>
    /// COpies the template of ADR and generate a new record in dedicated folder
    /// </summary>
    /// <param name="progressive"></param>
    /// <param name="shortName"></param>
    /// <param name="adrRootPath"></param>
    /// <param name="repoRootPath"></param>
    /// <param name="templateName"></param>
    void CopyFromTemplate(int progressive, string shortName, string adrRootPath, string repoRootPath, string templateName)
    {
        using (this._Logger.BeginScope(CreateScope(nameof(CopyFromTemplate))))
        {
            ArgumentNullException.ThrowIfNullOrEmpty(shortName);
            ArgumentNullException.ThrowIfNullOrEmpty(adrRootPath);
            ArgumentNullException.ThrowIfNullOrEmpty(repoRootPath);
            ArgumentNullException.ThrowIfNullOrEmpty(templateName);

            // preparing folder
            string folderName = progressive.ToString("0000") + "-" + shortName;
            this._Logger.LogInformation("Creating target folder for record: {targetRecordFolder}", folderName);
            Directory.CreateDirectory(Path.Combine(adrRootPath, folderName));

            // copying template
            this._Logger.LogInformation("Copying template {Template} to create new record", folderName);
            var targetFilePath = Path.Combine(adrRootPath, folderName, folderName + ".md");
            File.Copy(Path.Combine(repoRootPath, "templates", "adr", templateName), targetFilePath);
            Directory.CreateDirectory(Path.Combine(adrRootPath, folderName, "img"));

            // set values of variables
            var content = File.ReadAllText(targetFilePath);
            content = content.Replace("$(Today)", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            File.WriteAllText(targetFilePath, content);
        }
    }

    void AddNewRecordToRegistry(Record item, string adrRootPath)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNullOrEmpty(adrRootPath);

        StringBuilder newRow = new StringBuilder();
        //Progressive; short name; Creation Date;
        newRow.Append(item.Progressive.ToString("0000") ?? "").Append(";");
        newRow.Append(item.ShortName).Append(";");
        newRow.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).Append(";");
        newRow.Append("Proposed");
        File.AppendAllLines(Path.Combine(adrRootPath, "adr.csv"), new string[] { newRow.ToString() });
    }

    #endregion

    /// <summary>
    /// Checks existing registry and return next available Id
    /// </summary>
    /// <param name="adrRootPath"></param>
    /// <returns></returns>
    public int CalculateNextRecordId(string adrRootPath)
    {
        using (this._Logger.BeginScope(CreateScope(nameof(CalculateNextRecordId))))
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
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public int AddNewRecordToRegistry(string shortName, string repoRootPath, string templateName)
    {
        using (this._Logger.BeginScope(CreateScope(nameof(AddNewRecordToRegistry))))
        {
            ArgumentNullException.ThrowIfNullOrEmpty(shortName);
            ArgumentNullException.ThrowIfNullOrEmpty(repoRootPath);
            ArgumentNullException.ThrowIfNullOrEmpty(templateName);

            // create folder tree
            var adrRootPath = Path.Combine(repoRootPath, "docs", "adr");
            if (!Directory.Exists(adrRootPath))
            {
                this._Logger.LogInformation("Creating target folder for ADR: /docs/adr");
                Directory.CreateDirectory(adrRootPath);
            }

            // calculating the progressive
            var adrRootDirectory = new DirectoryInfo(adrRootPath);
            var progressive = this.CalculateNextRecordId(adrRootPath);

            // Creating new item from template
            this.CopyFromTemplate(progressive, shortName, adrRootPath, repoRootPath, templateName);

            // update registry
            var item = new Record()
            {
                Progressive = progressive,
                Repository = repoRootPath,
                TemplateName = templateName,
                ShortName = shortName
            };
            this.AddNewRecordToRegistry(item, adrRootPath);

            return progressive;
        }
    }
}
