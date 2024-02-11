namespace DotnetMarkdownReports.Tests;

internal sealed class Output
{
    public bool HasWrite { get; private set; }

    public string Last { get; private set; } = string.Empty;

    public void Write(string s)
    {
        Last = s;
        HasWrite = true;
    }
}
