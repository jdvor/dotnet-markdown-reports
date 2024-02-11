namespace DotnetMarkdownReports.Tests;

public sealed class DataFixture : IDisposable
{
    public DirectoryInfo TrxDataDir { get; }

    public DirectoryInfo EmptyTrxDataDir { get; }

    public DirectoryInfo RoslynatorDataDir { get; }

    public DirectoryInfo EmptyRoslynatorDataDir { get; }

    public DirectoryInfo CoberturaDataDir { get; }

    public DirectoryInfo EmptyCoberturaDataDir { get; }

    private readonly string tempDir;
    private bool disposed;

    public DataFixture()
    {
        var testDir = GetTestDirOrThrow();
        TrxDataDir = GetDataDirOrThrow(testDir, "trx");
        RoslynatorDataDir = GetDataDirOrThrow(testDir, "roslynator");
        CoberturaDataDir = GetDataDirOrThrow(testDir, "cobertura");

        tempDir = GetTempDir();
        EmptyTrxDataDir = CreateEmptyDir(tempDir, "trx");
        EmptyRoslynatorDataDir = CreateEmptyDir(tempDir, "roslynator");
        EmptyCoberturaDataDir = CreateEmptyDir(tempDir, "cobertura");
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        try
        {
            Directory.Delete(tempDir, recursive: true);
        }
        catch
        {
            // yes
        }
    }

    private static DirectoryInfo GetTestDirOrThrow()
    {
        const int maxDepth = 6;
        var depth = 0;
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir.Parent is not null && depth < maxDepth)
        {
            dir = dir.Parent;
            if (dir.Name == "test")
            {
                return dir;
            }

            ++depth;
        }

        throw new InvalidOperationException("could not find test directory");
    }

    private static DirectoryInfo GetDataDirOrThrow(DirectoryInfo baseDir, string suffix)
    {
        var dir = new DirectoryInfo(Path.Join(baseDir.FullName, "data", suffix));
        return dir.Exists
            ? dir
            : throw new InvalidOperationException($"could not find {suffix} test data directory");
    }

    private static string GetTempDir()
    {
        var path = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString("N").Substring(0, 8));
        Directory.CreateDirectory(path);
        return path;
    }

    private static DirectoryInfo CreateEmptyDir(string baseDir, string suffix)
    {
        var dir = new DirectoryInfo(Path.Join(baseDir, suffix));
        dir.Create();
        return dir;
    }
}
