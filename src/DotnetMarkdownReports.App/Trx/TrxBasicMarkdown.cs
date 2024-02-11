namespace DotnetMarkdownReports.App.Trx;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static DotnetMarkdownReports.App.Constants;
using static TestExecResult;

internal class TrxBasicMarkdown : IAction
{
    private const int TerseFailedLimit = 100;
    private readonly CommonParameters common;
    private readonly Action<string>? testOutput;

    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, $"{RootNs}.CommonParameters", AsmName)]
    public TrxBasicMarkdown(CommonParameters common)
    {
        this.common = common;
    }

    internal TrxBasicMarkdown(CommonParameters common, Action<string> testOutput)
        : this(common)
    {
        this.testOutput = testOutput;
    }

    public int Execute()
    {
        if (!Util.TryGetSourceFiles(common.SourcePath, "trx", out var files))
        {
            return 0;
        }

        var tmp = new Dictionary<string, TestExec>();

        foreach (var fileInfo in files)
        {
            var tr = TestRun.ReadFromFile(fileInfo);
            AppendTestRun(tmp, tr);
        }

        if (tmp.Count == 0)
        {
            return 0;
        }

        var writeHeader = !string.IsNullOrEmpty(common.OutputFilePath);
        var sb = new StringBuilder();
        WriteMarkdown(sb, tmp.Values, writeHeader);
        WriteOutput(sb, common.OutputFilePath);

        return 0;
    }

    private static void AppendTestRun(Dictionary<string, TestExec> acc, TestRun tr)
    {
        static string TheoryArgs(string name)
        {
            var idx = name.IndexOf('(');
            return idx > 0
                ? name.Substring(idx + 1, name.Length - idx - 2)
                : string.Empty;
        }

        foreach (var r in tr.Results)
        {
            var def = tr.TestDefinitions.FirstOrDefault(x => x.Id == r.TestId);
            if (def?.TestMethod is null)
            {
                continue;
            }

            var (ns, @class) = GetNamespaceAndClass(def.TestMethod.ClassName);
            var full = $"{def.TestMethod.ClassName}.{def.Name}";
            if (acc.TryGetValue(full, out var existing) && existing.EndTime >= r.EndTime)
            {
                continue;
            }

            var @short = $"{@class}.{def.TestMethod.Name}";
            var targs = TheoryArgs(def.Name);
            var err = r.Output?.ErrorInfo?.Message ?? string.Empty;
            var stack = r.Output?.ErrorInfo?.StackTrace ?? string.Empty;
            var outcome = r.Outcome.ToLowerInvariant();
            var testExec = outcome switch
            {
                "passed" => new TestExec(full, @short, @class, ns, targs, Passed, r.Duration, r.EndTime),
                "failed" => new TestExec(full, @short, @class, ns, targs, Failed, r.Duration, r.EndTime, err, stack),
                "notexecuted" => new TestExec(full, @short, @class, ns, targs, Skipped, r.Duration, r.EndTime, err),
                _ => new TestExec(full, @short, @class, ns, targs, Other, r.Duration, r.EndTime, err, stack),
            };
            acc[full] = testExec;
        }
    }

    private static (string ns, string className) GetNamespaceAndClass(string fullClassName)
    {
        var lastDotIndex = fullClassName.LastIndexOf('.');
        return lastDotIndex <= 0 || lastDotIndex == fullClassName.Length - 1
            ? (string.Empty, fullClassName)
            : (fullClassName[..lastDotIndex], fullClassName[(lastDotIndex + 1)..]);
    }

    private static void WriteMarkdown(StringBuilder sb, ICollection<TestExec> tests, bool writeHeader)
    {
        if (writeHeader)
        {
            sb.H1("Tests");
        }

        var passed = tests.Count(x => x.Result == Passed);
        var failed = tests.Count(x => x.Result == Failed);
        var skipped = tests.Count(x => x.Result == Skipped);
        var status = failed > 0 || passed < 1 ? "Fail" : "Ok";
        var testsByAssembly = tests.GroupBy(x => x.Namespace)
            .ToImmutableSortedDictionary(x => x.Key, x => x.ToImmutableArray());

        sb.H2("Tests Summary");
        var mt1 = new MarkdownTable(["status", "failed", "passed", "skipped"], 1);
        mt1.AddRow(status, failed.ToString(), passed.ToString(), skipped.ToString());
        sb.Table(mt1);

        sb.H2("Test Results By Namespace");
        sb.Details(false, "by namespace", b =>
        {
            var mt2 = new MarkdownTable(["namespace", "status", "failed", "passed", "skipped"], testsByAssembly.Count);
            foreach (var (ns, testsAsm) in testsByAssembly)
            {
                passed = testsAsm.Count(x => x.Result == Passed);
                failed = testsAsm.Count(x => x.Result == Failed);
                skipped = testsAsm.Count(x => x.Result == Skipped);
                status = failed > 0 || passed < 1 ? "Fail" : "Ok";
                mt2.AddRow(ns, status, failed.ToString(), passed.ToString(), skipped.ToString());
            }

            b.Table(mt2);
        });

        if (failed > 0)
        {
            sb.H2("Failed Tests");
            var failedTests = tests.Where(x => x.Result == Failed);
            if (failed > TerseFailedLimit)
            {
                WriteTerseFailedTestsMarkdown(sb, failedTests);
            }
            else
            {
                WriteFailedTestsMarkdown(sb, failedTests);
            }
        }
    }

    private static void WriteFailedTestsMarkdown(StringBuilder sb, IEnumerable<TestExec> failedTests)
    {
        var failedTestsByShortName = failedTests
            .GroupBy(x => x.ShortName)
            .ToImmutableSortedDictionary(x => x.Key, x => x.ToArray());
        foreach (var (shortName, failedExecs) in failedTestsByShortName)
        {
            sb.H3(shortName);
            foreach (var fe in failedExecs)
            {
                if (!string.IsNullOrEmpty(fe.TheoryArgs))
                {
                    sb.AppendLine($"**arguments:** {fe.TheoryArgs}<br/>");
                }

                sb.AppendLine($"**error:** {fe.Error}");
                sb.AppendLine();
                if (!string.IsNullOrEmpty(fe.StackTrace))
                {
                    sb.Details(false, "stacktrace", b =>
                    {
                        b.AppendLine();
                        b.AppendLine("```");
                        b.AppendLine(fe.StackTrace);
                        b.AppendLine("```");
                        b.AppendLine();
                    });
                    sb.HorizontalRule();
                }
            }
        }
    }

    private static void WriteTerseFailedTestsMarkdown(StringBuilder sb, IEnumerable<TestExec> failedTests)
    {
        sb.AppendLine("*There are too many failed tests, therefore the details are not included.*");
        sb.AppendLine();
        foreach (var name in failedTests.Select(x => x.FullName).OrderBy(x => x))
        {
            sb.Append("* ");
            sb.AppendLine(name);
        }
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
