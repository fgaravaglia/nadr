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
    readonly IRecordRegistryRepositoryFactory _RepositoryFactory;
    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="repoFactory">
    /// <exception cref="ArgumentNullException"></exception>
    public AdlService(ILogger logger, IRecordRegistryRepositoryFactory repoFactory)
    {
        this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._RepositoryFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
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

    IRecordRegistryRepository InstanceFromRootOfRepository(string repoRootPath)
    {
        return this._RepositoryFactory.InstanceForCsv(GetAdrRootPath(repoRootPath), "adr.csv");
    }

    static string GetAdrRootPath(string repoRootPath)
    {
        return Path.Combine(repoRootPath, "docs", "adr");
    }

    static string GetItemFolderName(IRegistryRecord record)
    {
        return record.Progressive.ToString("0000") + "-" + record.ShortName;
    }

    void UpdateStatusOnMdFile(IRegistryRecord record, string repoRootPath)
    {
        this._Logger.LogInformation("Updating Status on MD file...");
        // check if file exists
        var mdFilePath = Path.Combine(GetAdrRootPath(repoRootPath), GetItemFolderName(record), GetItemFolderName(record) + ".md");
        if (!File.Exists(mdFilePath))
            throw new InvalidOperationException($"Unable to find {GetItemFolderName(record)}.md File");

        // read content and update text
        var text = File.ReadAllText(mdFilePath);
        text.Replace("| Proposed |", $"|{record.Status}|");
        text.Replace($"| {Record.StatusEnum.Proposed.ToString()} |", $"|{record.Status}|");
        text.Replace($"| {Record.StatusEnum.Accepted.ToString()} |", $"|{record.Status}|");
        text.Replace($"| {Record.StatusEnum.Deprecated.ToString()} |", $"|{record.Status}|");
        text.Replace($"| {Record.StatusEnum.Superseded.ToString()} |", $"|{record.Status}|");
        File.WriteAllText(mdFilePath, text);
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
            var adrRootPath = GetAdrRootPath(repoRootPath);
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
            var repository = InstanceFromRootOfRepository(repoRootPath);
            item.CreatedOn = DateTime.Now;
            item.Status = Record.StatusEnum.Proposed;
            item.Progressive = repository.Save(item);

            return progressive;
        }
    }
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void ApproveRecord(int progressive, string repoRootPath)
    {
        using (this._Logger.BeginScope(CreateScope(nameof(ApproveRecord))))
        {
            if (progressive <= 0)
                throw new ArgumentOutOfRangeException(nameof(progressive), "Record id must be greater than zero");
            ArgumentNullException.ThrowIfNullOrEmpty(repoRootPath);

            var repository = InstanceFromRootOfRepository(repoRootPath);
            IRegistryRecord? existing = repository.GetById(progressive);
            if (existing == null)
                throw new InvalidOperationException($"Unable to find record {progressive}");

            // updating it
            this._Logger.LogInformation("Updating record...");
            existing.Status = Record.StatusEnum.Accepted;
            repository.Save(existing);

            // update .md file
            UpdateStatusOnMdFile(existing, repoRootPath);
        }
    }
    /// <summary>
    /// deprecates the record
    /// </summary>
    /// <param name="progressive"></param>
    /// <param name="repoRootPath"></param>
    public void DeprecateRecord(int progressive, string repoRootPath)
    {
        using (this._Logger.BeginScope(CreateScope(nameof(DeprecateRecord))))
        {
            if (progressive <= 0)
                throw new ArgumentOutOfRangeException(nameof(progressive), "Record id must be greater than zero");
            ArgumentNullException.ThrowIfNullOrEmpty(repoRootPath);

            var repository = InstanceFromRootOfRepository(repoRootPath);
            IRegistryRecord? existing = repository.GetById(progressive);
            if (existing == null)
                throw new InvalidOperationException($"Unable to find record {progressive}");

            // updating it
            this._Logger.LogInformation("Updating record...");
            existing.Status = Record.StatusEnum.Deprecated;
            repository.Save(existing);

            // update .md file
            UpdateStatusOnMdFile(existing, repoRootPath);
        }
    }
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void SupersedRecord(int progressive, string repoRootPath, int replacingProgressive)
    {
        using (this._Logger.BeginScope(CreateScope(nameof(SupersedRecord))))
        {
            ArgumentNullException.ThrowIfNullOrEmpty(repoRootPath);
            if (progressive <= 0)
                throw new ArgumentOutOfRangeException(nameof(progressive), "Record id must be greater than zero");
            if (replacingProgressive <= 0)
                throw new ArgumentOutOfRangeException(nameof(replacingProgressive), "Replacing record must be specified");

            var repository = InstanceFromRootOfRepository(repoRootPath);
            IRegistryRecord? existing = repository.GetById(progressive);
            if (existing == null)
                throw new InvalidOperationException($"Unable to find record {progressive}");

            // updating it
            this._Logger.LogInformation("Updating record...");
            existing.Status = Record.StatusEnum.Superseded;
            existing.Links.Add($"[ReferencedBy:{progressive}]");
            repository.Save(existing);

            // update .md file
            UpdateStatusOnMdFile(existing, repoRootPath);
        }
    }
}
