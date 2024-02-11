namespace DotnetMarkdownReports.App;

internal sealed class Err
{
    private readonly bool enabled;

    public Err(CommonParameters common)
    {
        enabled = !common.Quiet;
    }

    internal void Write(string msg)
    {
        if (enabled)
        {
            Console.Error.Write(msg);
        }
    }

    internal void WriteLine(string msg)
    {
        if (enabled)
        {
            Console.Error.WriteLine(msg);
        }
    }
}
