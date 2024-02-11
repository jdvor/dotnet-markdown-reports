namespace DotnetMarkdownReports.App.Roslynator;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using static DotnetMarkdownReports.App.Constants;

internal class RoslynatorBasicMarkdown : IAction
{
    private readonly CommonParameters common;
    private readonly Action<string>? testOutput;

    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, $"{RootNs}.CommonParameters", AsmName)]
    public RoslynatorBasicMarkdown(CommonParameters common)
    {
        this.common = common;
    }

    internal RoslynatorBasicMarkdown(CommonParameters common, Action<string> testOutput)
        : this(common)
    {
        this.testOutput = testOutput;
    }

    public int Execute()
    {
        if (!Util.TryGetSourceFiles(common.SourcePath, "roslynator.xml", out var files))
        {
            return 0;
        }

        var newestFile = files.OrderBy(x => x.LastAccessTimeUtc).First();
        var report = RoslynatorReport.ReadFromFile(newestFile);
        var writeHeader = !string.IsNullOrEmpty(common.OutputFilePath);
        var sb = new StringBuilder();
        WriteMarkdown(sb, report.CodeAnalysis, writeHeader);
        WriteOutput(sb, common.OutputFilePath);

        return 0;
    }

    private static void WriteMarkdown(StringBuilder sb, CodeAnalysis analysis, bool writeHeader)
    {
        if (writeHeader)
        {
            sb.H1("Code Analysis");
        }

        if (analysis.Summary.Length == 0)
        {
            sb.AppendLine("No issues found.");
            return;
        }

        sb.H2("Code Analysis Summary");

        var mt1 = new MarkdownTable(["id", "count", "title", "description"], analysis.Summary.Length);
        foreach (var diagnostic in analysis.Summary)
        {
            var id = string.IsNullOrEmpty(diagnostic.HelpLink)
                ? diagnostic.Id
                : $"[{diagnostic.Id}]({diagnostic.HelpLink})";
            var count = diagnostic.Count.ToString(CultureInfo.InvariantCulture);
            mt1.AddRow(id, count, diagnostic.Title, diagnostic.Description.EscapeMarkdownTableCell());
        }

        sb.Table(mt1);

        sb.Details(false, "code analysis issues by project", b =>
        {
            foreach (var project in analysis.Projects)
            {
                b.H3(project.Name);
                var mt2 = new MarkdownTable(["id", "path"], project.Diagnostics.Length);
                foreach (var pdiag in project.Diagnostics)
                {
                    var path = pdiag.FilePath.LeftTrimSourceCodePath("src/");
                    path = $"{path} L{pdiag.Location.Line}:{pdiag.Location.Character}";
                    mt2.AddRow(pdiag.Id, path);
                }

                b.Table(mt2);
            }
        });
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
