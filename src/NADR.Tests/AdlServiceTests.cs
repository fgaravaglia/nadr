using System.Globalization;
using Microsoft.Extensions.Logging;
using NADR.Domain;
using NSubstitute;

namespace NADR.Tests;

public class AdlServiceTests
{
    AdlService _Service;
    string _WorkingFolder;

    [SetUp]
    public void Setup()
    {
        this._WorkingFolder = Path.Combine(Environment.CurrentDirectory, nameof(AdlServiceTests));

        // generating template
        var templateFolder = Path.Combine(this._WorkingFolder, "templates", "adr");
        if (Directory.Exists(templateFolder))
            Directory.Delete(templateFolder, true);
        Directory.CreateDirectory(templateFolder);
        File.WriteAllText(Path.Combine(templateFolder, "garavaglia.md"), "this is my template");

        this._Service = new AdlService(Substitute.For<ILogger>());
    }

    [Test]
    public void AddNewRecordToRegistry_CreatesAdlFolders()
    {
        //*************GIVEN
        //delete target folder for adr
        var adrRootFolder = Path.Combine(this._WorkingFolder, "docs", "adr");
        if (Directory.Exists(adrRootFolder))
            Directory.Delete(adrRootFolder, true);

        //*************WHEN
        int id = this._Service.AddNewRecordToRegistry("Test", this._WorkingFolder, "garavaglia.md");

        //*************ASSERT
        Assert.That(Directory.Exists(adrRootFolder), Is.True);
        Assert.Pass();
    }

    [Test]
    public void AddNewRecordToRegistry_Returns_OneIfNoRecordExists()
    {
        //*************GIVEN
        //delete target folder for adr
        var adrRootFolder = Path.Combine(this._WorkingFolder, "docs", "adr");
        if (Directory.Exists(adrRootFolder))
            Directory.Delete(adrRootFolder, true);

        //*************WHEN
        int id = this._Service.AddNewRecordToRegistry("Test", this._WorkingFolder, "garavaglia.md");

        //*************ASSERT
        Assert.That(Directory.Exists(adrRootFolder), Is.True);
        Assert.That(id, Is.EqualTo(1));
        Assert.Pass();
    }

    [Test]
    public void AddNewRecordToRegistry_Returns_LastPlusOneIfRecordExists()
    {
        //*************GIVEN
        //delete target folder for adr
        var adrRootFolder = Path.Combine(this._WorkingFolder, "docs", "adr", "0001-test1");
        if (!Directory.Exists(adrRootFolder))
            Directory.CreateDirectory(adrRootFolder);

        //*************WHEN
        int id = this._Service.AddNewRecordToRegistry("Test", this._WorkingFolder, "garavaglia.md");

        //*************ASSERT
        Assert.That(id, Is.EqualTo(2));
        Assert.Pass();
    }

    [Test]
    public void AddNewRecordToRegistry_Creates_RecordFolder()
    {
        //*************GIVEN
        //delete target folder for adr
        var adrRootFolder = Path.Combine(this._WorkingFolder, "docs", "adr", "0001-test1");
        if (!Directory.Exists(adrRootFolder))
            Directory.CreateDirectory(adrRootFolder);

        //*************WHEN
        int id = this._Service.AddNewRecordToRegistry("Test", this._WorkingFolder, "garavaglia.md");

        //*************ASSERT
        var targetRecordFolder = Path.Combine(this._WorkingFolder, "docs", "adr", "0002-Test");
        Assert.That(Directory.Exists(targetRecordFolder), Is.True);
        Assert.That(File.Exists(Path.Combine(targetRecordFolder, "0002-Test.md")), Is.True);
        Assert.That(Directory.Exists(Path.Combine(targetRecordFolder, "img")), Is.True);
        Assert.Pass();
    }

    [Test]
    public void AddNewRecordToRegistry_Appends_NewLineIntoRegistryFile()
    {
        //*************GIVEN
        //delete target folder for adr
        var adrRootFolder = Path.Combine(this._WorkingFolder, "docs", "adr", "0001-test1");
        if (!Directory.Exists(adrRootFolder))
            Directory.CreateDirectory(adrRootFolder);
        var registryFilePath = Path.Combine(this._WorkingFolder, "docs", "adr", "adr.csv");
        if (File.Exists(registryFilePath))
            File.Delete(registryFilePath);
        var initialLines = 0;

        //*************WHEN
        int id = this._Service.AddNewRecordToRegistry("Test", this._WorkingFolder, "garavaglia.md");

        //*************ASSERT
        var actualLines = File.ReadAllLines(registryFilePath).Count();
        Assert.That(actualLines, Is.EqualTo(initialLines + 1), $"Wrong File content: Found <{File.ReadAllText(registryFilePath)}>");
        Assert.Pass();
    }

    [Test]
    public void AddNewRecordToRegistry_Appends_ExpectedLine()
    {
        //*************GIVEN
        //delete target folder for adr
        var adrRootFolder = Path.Combine(this._WorkingFolder, "docs", "adr", "0001-test1");
        if (!Directory.Exists(adrRootFolder))
            Directory.CreateDirectory(adrRootFolder);
        var registryFilePath = Path.Combine(this._WorkingFolder, "docs", "adr", "adr.csv");
        if (File.Exists(registryFilePath))
            File.Delete(registryFilePath);

        //*************WHEN
        int id = this._Service.AddNewRecordToRegistry("Test", this._WorkingFolder, "garavaglia.md");

        //*************ASSERT
        Assert.That(id, Is.EqualTo(2));
        var lastLine = File.ReadAllLines(registryFilePath).First().Split(';').ToList();
        Assert.That(lastLine, Has.Count.EqualTo(4), $"The CSV contains wrong line. Found <{String.Join(";", lastLine)}>");
        Assert.That(lastLine[0], Is.EqualTo("0002"), $"The Column 0 contains wrong data. Found <{lastLine[0]}> instead of <0002>");
        Assert.That(lastLine[1], Is.EqualTo("Test"), $"The Column 1 contains wrong data. Found <{lastLine[1]}> instead of <Test>");
        Assert.That(lastLine[2].StartsWith(DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)), Is.True,
                    $"The Column 2 contains wrong data. Found <{lastLine[2]}> instead of <Today>");
        Assert.That(lastLine[3], Is.EqualTo("Proposed"), $"The Column 3 contains wrong data. Found <{lastLine[3]}> instead of <Proposed>");
        Assert.Pass();
    }

}