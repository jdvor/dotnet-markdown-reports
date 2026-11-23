namespace DotnetMarkdownReports.App.Sarif;

using DotnetMarkdownReports.App.Roslynator;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using static DotnetMarkdownReports.App.Constants;

internal class SarifBasicMarkdown : IAction
{
    private readonly CommonParameters common;
    private readonly string level;
    private readonly Action<string>? testOutput;

    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, $"{RootNs}.CommonParameters", AsmName)]
    public SarifBasicMarkdown(CommonParameters common, string level)
    {
        this.common = common;
        this.level = level;
    }

    internal SarifBasicMarkdown(CommonParameters common, string level, Action<string> testOutput)
        : this(common, level)
    {
        this.testOutput = testOutput;
    }

    public int Execute()
    {
        if (!SarifParser.TryParseLevel(level, out var minLevelRank))
        {
            new Err(common).WriteLine(
                $"Invalid level '{level}', expected one of: {string.Join(", ", SarifParser.Levels)}.");
            return 1;
        }

        if (!Util.TryGetSourceFiles(common.SourcePath, "*.sarif", out var files))
        {
            return 0;
        }

        var logs = new List<(string, SarifLog)>(files.Count);
        foreach (var file in files)
        {
            try
            {
                logs.Add((Path.GetFileNameWithoutExtension(file.Name), SarifParser.ReadFromFile(file)));
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                new Err(common).WriteLine($"Failed to read SARIF log '{file.FullName}': {ex.Message}");
                return 1;
            }
        }

        var analysis = SarifParser.ToCodeAnalysis(logs, minLevelRank);
        var writeHeader = !string.IsNullOrEmpty(common.OutputFilePath);
        var sb = new StringBuilder();
        RoslynatorBasicMarkdown.WriteMarkdown(sb, analysis, writeHeader);
        WriteOutput(sb, common.OutputFilePath);

        return 0;
    }

    private void WriteOutput(StringBuilder sb, string outputFilePath)
    {
        if (testOutput is not null)
        {
            testOutput(sb.ToString());
            return;
        }

        if (string.IsNullOrEmpty(outputFilePath))
        {
            Console.Write(sb.ToString());
            return;
        }

        var opts = new FileStreamOptions { Mode = FileMode.Create, Access = FileAccess.Write, Share = FileShare.None };
        using var writer = new StreamWriter(outputFilePath, Encoding.UTF8, opts);
        writer.Write(sb.ToString());
    }
}
