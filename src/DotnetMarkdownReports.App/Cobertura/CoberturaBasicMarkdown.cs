namespace DotnetMarkdownReports.App.Cobertura;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using static DotnetMarkdownReports.App.Constants;

internal class CoberturaBasicMarkdown : IAction
{
    private readonly CommonParameters common;
    private readonly Action<string>? testOutput;

    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, $"{RootNs}.CommonParameters", AsmName)]
    public CoberturaBasicMarkdown(CommonParameters common)
    {
        this.common = common;
    }

    internal CoberturaBasicMarkdown(CommonParameters common, Action<string> testOutput)
        : this(common)
    {
        this.testOutput = testOutput;
    }

    public int Execute()
    {
        if (!Util.TryGetSourceFiles(common.SourcePath, "coverage.cobertura.xml", out var files))
        {
            return 0;
        }

        var projects = MergeReports(files);

        var writeHeader = !string.IsNullOrEmpty(common.OutputFilePath);
        var sb = new StringBuilder();
        WriteMarkdown(sb, projects, writeHeader);
        WriteOutput(sb, common.OutputFilePath);

        return 0;
    }

    private static Package[] MergeReports(List<FileInfo> files)
    {
        var tmp = new Dictionary<string, (Package Package, int Timestamp)>();
        foreach (var fileInfo in files)
        {
            var cov = Coverage.ReadFromFile(fileInfo);
            foreach (var pkg in cov.Packages)
            {
                if (tmp.TryGetValue(pkg.Name, out var v))
                {
                    if (v.Timestamp < cov.Timestamp)
                    {
                        tmp[pkg.Name] = (pkg, cov.Timestamp);
                    }
                }
                else
                {
                    tmp[pkg.Name] = (pkg, cov.Timestamp);
                }
            }
        }

        return tmp.Values.Select(x => x.Package).ToArray();
    }

    private static void WriteMarkdown(StringBuilder sb, Package[] projects, bool writeHeader)
    {
        if (writeHeader)
        {
            sb.H1("Test Coverage");
        }

        sb.H2("Test Coverage by Project");

        var mt1 = new MarkdownTable(["project", "coverage"], projects.Length);
        foreach (var pkg in projects)
        {
            var coverage = Math.Round(pkg.LineRate * 100, 0).ToString(CultureInfo.InvariantCulture);
            mt1.AddRow(pkg.Name, $"{coverage} %");
        }

        sb.Table(mt1);
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
