namespace DotnetMarkdownReports.App;

internal sealed class Std
{
    private readonly bool enabled;

    public Std(CommonParameters common)
    {
        enabled = !common.Quiet;
    }

    internal void Write(string msg)
    {
        if (enabled)
        {
            Console.Write(msg);
        }
    }

    internal void WriteLine(string msg)
    {
        if (enabled)
        {
            Console.WriteLine(msg);
        }
    }
}
