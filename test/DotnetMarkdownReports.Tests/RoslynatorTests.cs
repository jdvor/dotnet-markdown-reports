namespace DotnetMarkdownReports.Tests;

using DotnetMarkdownReports.App;
using DotnetMarkdownReports.App.Roslynator;

public class RoslynatorTests : IClassFixture<DataFixture>
{
    private readonly DataFixture fixture;

    public RoslynatorTests(DataFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Roslynator_data_parsed_correctly()
    {
        var common = new CommonParameters(fixture.RoslynatorDataDir.FullName);
        var output = new Output();
        var sut = new RoslynatorBasicMarkdown(common, output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.True(output.HasWrite, "some output has been written");
        Assert.False(string.IsNullOrEmpty(output.Last), "the output is not empty");
        Assert.Contains("## Code Analysis Summary", output.Last);
        Assert.Contains("<summary>code analysis issues by project</summary>", output.Last);
        Assert.Contains("CS8019", output.Last);
        Assert.Contains("SA1507", output.Last);
    }

    [Fact]
    public void Does_not_fail_when_source_is_missing()
    {
        var common = new CommonParameters(fixture.EmptyRoslynatorDataDir.FullName);
        var output = new Output();
        var sut = new RoslynatorBasicMarkdown(common, output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.False(output.HasWrite, "the is no output");
    }
}
