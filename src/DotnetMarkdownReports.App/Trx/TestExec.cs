namespace DotnetMarkdownReports.App.Trx;

public sealed record TestExec(
    string Version,
    string FullName,
    string ShortName,
    string ClassName,
    string Namespace,
    string TheoryArgs,
    TestExecResult Result,
    TimeSpan Duration,
    DateTimeOffset EndTime,
    string Error = "",
    string StackTrace = "")
{
    public override string ToString()
        => $"{ShortName}: {Result}";
}
