#pragma warning disable SA1402

namespace DotnetMarkdownReports.App.Trx;

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using static DotnetMarkdownReports.App.Constants;

[XmlRoot("TestRun", Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
public class TestRun
{
    public Times Times { get; set; } = new();

    [XmlArray]
    [XmlArrayItem(ElementName = "UnitTestResult")]
    public UnitTestResult[] Results { get; set; } = Array.Empty<UnitTestResult>();

    [XmlArray]
    [XmlArrayItem(ElementName = "UnitTest")]
    public UnitTest[] TestDefinitions { get; set; } = Array.Empty<UnitTest>();

    [XmlArray]
    [XmlArrayItem(ElementName = "TestEntry")]
    public TestEntry[] TestEntries { get; set; } = Array.Empty<TestEntry>();

    [XmlArray]
    [XmlArrayItem(ElementName = "TestList")]
    public TestList[] TestLists { get; set; } = Array.Empty<TestList>();

    public ResultSummary ResultSummary { get; set; } = new();

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.TestRun", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.Times", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.UnitTestResult", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.FailedTestOutput", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.UnitTest", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.Execution", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.TestMethod", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.TestEntry", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.TestList", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.ResultSummary", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Trx.Counters", AsmName)]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "using DynamicDependency attributes")]
    internal static TestRun ReadFromFile(FileInfo file)
    {
        using var stream = file.OpenRead();
        using var reader = new XmlTextReader(stream);
        var serializer = new XmlSerializer(typeof(TestRun));
        return (TestRun)serializer.Deserialize(reader)!;
    }
}

public class Times
{
    [XmlAttribute("creation")]
    public string Creation { get; set; } = string.Empty;

    [XmlAttribute("queuing")]
    public string Queuing { get; set; } = string.Empty;

    [XmlAttribute("start")]
    public string Start { get; set; } = string.Empty;

    [XmlAttribute("finish")]
    public string Finish { get; set; } = string.Empty;
}

public class UnitTestResult
{
    [XmlAttribute("executionId")]
    public Guid ExecutionId { get; set; }

    [XmlAttribute("testId")]
    public Guid TestId { get; set; }

    [XmlAttribute("testName")]
    public string TestName { get; set; } = string.Empty;

    [XmlAttribute("duration")]
    public string DurationString { get; set; } = string.Empty;

    [XmlIgnore]
    public TimeSpan Duration => string.IsNullOrEmpty(DurationString) ? TimeSpan.Zero : TimeSpan.Parse(DurationString);

    [XmlAttribute("endTime")]
    public string EndTimeString { get; set; } = string.Empty;

    [XmlIgnore]
    public DateTimeOffset EndTime => string.IsNullOrEmpty(EndTimeString)
        ? DateTimeOffset.MinValue
        : DateTimeOffset.Parse(EndTimeString);

    [XmlAttribute("testType")]
    public string TestType { get; set; } = string.Empty;

    [XmlAttribute("outcome")]
    public string Outcome { get; set; } = string.Empty;

    [XmlAttribute("testListId")]
    public Guid TestListId { get; set; }

    [XmlAttribute("relativeResultsDirectory")]
    public string RelativeResultsDirectory { get; set; } = string.Empty;

    public FailedTestOutput? Output { get; set; }
}

public class FailedTestOutput
{
    public Info? ErrorInfo { get; set; }

    public class Info
    {
        public string Message { get; set; } = string.Empty;

        public string StackTrace { get; set; } = string.Empty;
    }
}

public class UnitTest
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("id")]
    public Guid Id { get; set; }

    public Execution? Execution { get; set; }

    public TestMethod? TestMethod { get; set; }
}

public class Execution
{
    [XmlAttribute("id")]
    public Guid Id { get; set; }
}

public class TestMethod
{
    [XmlAttribute("className")]
    public string ClassName { get; set; } = string.Empty;

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}

public class TestEntry
{
    [XmlAttribute("testId")]
    public Guid TestId { get; set; }

    [XmlAttribute("executionId")]
    public Guid ExecutionId { get; set; }

    [XmlAttribute("testListId")]
    public Guid TestListId { get; set; }
}

public class TestList
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("id")]
    public Guid Id { get; set; }
}

public class ResultSummary
{
    [XmlAttribute("outcome")]
    public string Outcome { get; set; } = string.Empty;

    public Counters Counters { get; set; } = new();
}

public class Counters
{
    [XmlAttribute("total")]
    public int Total { get; set; }

    [XmlAttribute("executed")]
    public int Executed { get; set; }

    [XmlAttribute("passed")]
    public int Passed { get; set; }

    [XmlAttribute("failed")]
    public int Failed { get; set; }

    [XmlAttribute("error")]
    public int Error { get; set; }

    [XmlAttribute("timeout")]
    public int Timeout { get; set; }

    [XmlAttribute("aborted")]
    public int Aborted { get; set; }

    [XmlAttribute("inconclusive")]
    public int Inconclusive { get; set; }

    [XmlAttribute("passedButRunAborted")]
    public int PassedButRunAborted { get; set; }

    [XmlAttribute("notRunnable")]
    public int NotRunnable { get; set; }

    [XmlAttribute("notExecuted")]
    public int NotExecuted { get; set; }

    [XmlAttribute("disconnected")]
    public int Disconnected { get; set; }

    [XmlAttribute("warning")]
    public int Warning { get; set; }

    [XmlAttribute("completed")]
    public int Completed { get; set; }

    [XmlAttribute("inProgress")]
    public int InProgress { get; set; }

    [XmlAttribute("pending")]
    public int Pending { get; set; }
}

#pragma warning restore SA1402
