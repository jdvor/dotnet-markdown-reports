namespace DotnetMarkdownReports.Tests;

using DotnetMarkdownReports.App;
using DotnetMarkdownReports.App.Trx;

public class TrxTests : IClassFixture<DataFixture>
{
    private readonly DataFixture fixture;

    public TrxTests(DataFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Trx_data_parsed_correctly()
    {
        var common = new CommonParameters(fixture.TrxDataDir.FullName);
        var output = new Output();
        var sut = new TrxBasicMarkdown(common, output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.True(output.HasWrite, "some output has been written");
        Assert.False(string.IsNullOrEmpty(output.Last), "the output is not empty");
        Assert.Contains("## Tests Summary", output.Last);
        Assert.Contains("<summary>stacktrace</summary>", output.Last);
    }

    [Fact]
    public void Does_not_fail_when_source_is_missing()
    {
        var common = new CommonParameters(fixture.EmptyTrxDataDir.FullName);
        var output = new Output();
        var sut = new TrxBasicMarkdown(common, output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.False(output.HasWrite, "the is no output");
    }
}
