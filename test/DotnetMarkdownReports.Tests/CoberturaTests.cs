namespace DotnetMarkdownReports.Tests;

using DotnetMarkdownReports.App;
using DotnetMarkdownReports.App.Cobertura;

public class CoberturaTests : IClassFixture<DataFixture>
{
    private readonly DataFixture fixture;

    public CoberturaTests(DataFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Cobertura_data_parsed_correctly()
    {
        var common = new CommonParameters(fixture.CoberturaDataDir.FullName);
        var output = new Output();
        var sut = new CoberturaBasicMarkdown(common, output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.True(output.HasWrite, "some output has been written");
        Assert.False(string.IsNullOrEmpty(output.Last), "the output is not empty");
        Assert.Contains("## Test Coverage by Project", output.Last);
        Assert.Contains("Terraverse.TestProject.SampleLib2", output.Last);
    }

    [Fact]
    public void Does_not_fail_when_source_is_missing()
    {
        var common = new CommonParameters(fixture.EmptyCoberturaDataDir.FullName);
        var output = new Output();
        var sut = new CoberturaBasicMarkdown(common, output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.False(output.HasWrite, "the is no output");
    }
}
