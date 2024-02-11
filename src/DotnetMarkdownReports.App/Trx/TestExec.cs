namespace DotnetMarkdownReports.App.Trx;

public record TestExec(
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
