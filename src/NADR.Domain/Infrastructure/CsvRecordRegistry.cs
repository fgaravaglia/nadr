
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using NADR.Domain.Models;

namespace NADR.Domain.Infrastructure
{
    /// <summary>
    /// implementation of registry based on CSV
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CsvRecordRegistry : IRecordRegistryRepository
    {
        const string TIMEPSTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss";

        #region Fields
        readonly string _FolderPath;
        readonly string _FileName;

        readonly object _Locker = new Object();
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CsvRecordRegistry(string path, string fileName)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            this._FileName = fileName;
            this._FolderPath = path;
        }
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IEnumerable<IRegistryRecord> GetAll()
        {
            var fileFullPath = Path.Combine(this._FolderPath, this._FileName);
            if (!File.Exists(fileFullPath))
                return new List<IRegistryRecord>();

            // read the content
            var fileRows = File.ReadAllLines(Path.Combine(this._FolderPath, this._FileName)).ToList();
            var records = new List<IRegistryRecord>();
            // parsing the rows
            foreach (var row in fileRows.Skip(1))
            {
                var columns = row.Split(';').ToList();
                if (columns.Count != 6)
                    throw new InvalidOperationException("Wrong line found: expected 4 columns but found " + columns.Count);
                var record = new Record();
                int index = 0;

                // parsing the columns of the line
                while (index < 6)
                {
                    switch (index)
                    {
                        case 0:
                            record.Progressive = int.Parse(columns[index]);
                            break;
                        case 1:
                            record.ShortName = columns[index];
                            break;
                        case 2:
                            record.CreatedOn = DateTime.ParseExact(columns[index], TIMEPSTAMP_FORMAT, CultureInfo.InvariantCulture);
                            break;
                        case 3:
                            record.Status = Enum.Parse<Record.StatusEnum>(columns[index]);
                            break;
                        case 4:
                            record.CurrentVersion = int.Parse(columns[index]);
                            break;
                        case 5:
                            record.Links = columns[index].Split('|').ToList();
                            break;
                        default:
                            throw new InvalidOperationException($"Column {index} not managed: {columns[index]}");
                    }
                    index++;
                }

                // parsing completed
                records.Add(record);
            }

            return records;
        }
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IRegistryRecord? GetById(int id)
        {
            return GetAll().SingleOrDefault(x => x.Progressive == id);
        }
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int Save(IRegistryRecord record)
        {
            ArgumentNullException.ThrowIfNull(record);
            if (record.Progressive <= 0)
                throw new ArgumentOutOfRangeException(nameof(record), "Progressive must be greater then zero");
            if (String.IsNullOrEmpty(record.ShortName))
                throw new ArgumentNullException(nameof(record), "Short Name cannot be null");
            var records = GetAll().ToList();

            // checking existing...
            var existing = records.SingleOrDefault(x => x.Progressive == record.Progressive);
            if (existing != null)
            {
                var index = records.IndexOf(existing);
                records.RemoveAt(index);
            }
            // add the item and save
            records.Add(record);
            WriteAll(records);

            return record.Progressive;
        }

        #region Private Methods

        void WriteAll(IEnumerable<IRegistryRecord> records)
        {
            StringBuilder newRow = new StringBuilder();
            newRow.AppendLine("Id;ShortName;CreationDate;Status;CurrentVersion;Links");

            foreach (var item in records)
            {
                //Progressive; short name; Creation Date;
                newRow.Append(item.Progressive.ToString("0000") ?? "").Append(";");
                newRow.Append(item.ShortName).Append(";");
                newRow.Append(item.CreatedOn.ToString(TIMEPSTAMP_FORMAT, CultureInfo.InvariantCulture)).Append(";");
                newRow.Append(item.Status.ToString()).Append(";");
                newRow.Append(item.CurrentVersion.ToString()).Append(";");
                newRow.AppendLine(String.Join("|", item.Links));
            }

            lock (this._Locker)
            {
                File.WriteAllText(Path.Combine(this._FolderPath, this._FileName), newRow.ToString());
            }
        }
        #endregion
    }
}