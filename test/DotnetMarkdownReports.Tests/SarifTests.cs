namespace DotnetMarkdownReports.Tests;

using DotnetMarkdownReports.App;
using DotnetMarkdownReports.App.Sarif;
using System.Text.RegularExpressions;

public class SarifTests : IClassFixture<DataFixture>
{
    private readonly DataFixture fixture;

    public SarifTests(DataFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Sarif_data_parsed_correctly()
    {
        var common = new CommonParameters(fixture.SarifDataDir.FullName);
        var output = new Output();
        var sut = new SarifBasicMarkdown(common, "note", output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.True(output.HasWrite, "some output has been written");
        Assert.Contains("## Code Analysis Summary", output.Last);
        Assert.Contains("<summary>code analysis issues by project</summary>", output.Last);
        Assert.Contains("### Sample.Core", output.Last);
        Assert.Contains("### Sample.Api", output.Last);
        Assert.DoesNotContain("### Sample.Clean", output.Last);
        Assert.Contains("[CA1716](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1716)", output.Last);
        Assert.Contains("Mark members as static", output.Last);
        Assert.Contains("src/Sample.Core/Worker.cs L40:9", output.Last);
        Assert.DoesNotContain("IDE0005", output.Last);
    }

    [Fact]
    public void Suppressed_results_are_skipped()
    {
        var common = new CommonParameters(fixture.SarifDataDir.FullName);
        var output = new Output();
        var sut = new SarifBasicMarkdown(common, "note", output.Write);

        sut.Execute();

        Assert.DoesNotContain("Worker.cs L52:9", output.Last);
        Assert.Matches(@"\| \[CA1848\]\([^)]+\) +\| 1 +\|", output.Last);
    }

    [Fact]
    public void Results_below_level_are_skipped()
    {
        var common = new CommonParameters(fixture.SarifDataDir.FullName);
        var output = new Output();
        var sut = new SarifBasicMarkdown(common, "warning", output.Write);

        sut.Execute();

        Assert.Contains("CA1848", output.Last);
        Assert.Contains("CS0168", output.Last);
        Assert.DoesNotContain("CA1822", output.Last);
        Assert.DoesNotContain("### Sample.Api", output.Last);
    }

    [Fact]
    public void No_issues_when_everything_is_filtered_out()
    {
        var common = new CommonParameters(fixture.SarifDataDir.FullName);
        var output = new Output();
        var sut = new SarifBasicMarkdown(common, "error", output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.Contains("No issues found.", output.Last);
    }

    [Fact]
    public void Fails_on_invalid_level()
    {
        var common = new CommonParameters(fixture.SarifDataDir.FullName, Quiet: true);
        var output = new Output();
        var sut = new SarifBasicMarkdown(common, "info", output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(1, exitCode);
        Assert.False(output.HasWrite, "there is no output");
    }

    [Fact]
    public void Does_not_fail_when_source_is_missing()
    {
        var common = new CommonParameters(fixture.EmptySarifDataDir.FullName);
        var output = new Output();
        var sut = new SarifBasicMarkdown(common, "note", output.Write);

        var exitCode = sut.Execute();

        Assert.Equal(0, exitCode);
        Assert.False(output.HasWrite, "there is no output");
    }

    [Fact]
    public void Logs_of_same_project_are_merged_and_deduplicated()
    {
        var common = new CommonParameters(fixture.SarifDataDir.FullName);
        var output = new Output();
        var sut = new SarifBasicMarkdown(common, "note", output.Write);

        sut.Execute();

        Assert.Single(Regex.Matches(output.Last, "### Sample.Multi"));
        Assert.Matches(@"\| \[CA1062\]\([^)]+\) +\| 2 +\|", output.Last);
        Assert.Single(Regex.Matches(output.Last, @"Parser\.cs L10:5"));
    }

    [Fact]
    public void Title_is_escaped_in_summary_table()
    {
        var common = new CommonParameters(fixture.SarifDataDir.FullName);
        var output = new Output();
        var sut = new SarifBasicMarkdown(common, "note", output.Write);

        sut.Execute();

        Assert.Contains("Prefer 'A &#124; B' over 'A &#124;&#124; B'", output.Last);
    }

    [Fact]
    public void Fails_on_invalid_sarif_log()
    {
        var dir = Directory.CreateTempSubdirectory();
        try
        {
            File.WriteAllText(Path.Join(dir.FullName, "Broken.sarif"), "{ broken");
            var common = new CommonParameters(dir.FullName, Quiet: true);
            var output = new Output();
            var sut = new SarifBasicMarkdown(common, "note", output.Write);

            var exitCode = sut.Execute();

            Assert.Equal(1, exitCode);
            Assert.False(output.HasWrite, "there is no output");
        }
        finally
        {
            dir.Delete(recursive: true);
        }
    }
}
