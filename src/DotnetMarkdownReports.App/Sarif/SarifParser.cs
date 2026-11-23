namespace DotnetMarkdownReports.App.Sarif;

using DotnetMarkdownReports.App.Roslynator;
using System.Text.Json;

internal static class SarifParser
{
    internal static readonly string[] Levels = ["none", "note", "warning", "error"];

    internal static bool TryParseLevel(string? value, out int rank)
    {
        rank = Array.IndexOf(Levels, value?.Trim().ToLowerInvariant());
        return rank >= 0;
    }

    internal static SarifLog ReadFromFile(FileInfo file)
    {
        using var stream = file.OpenRead();
        return JsonSerializer.Deserialize(stream, SarifJsonContext.Default.SarifLog) ?? new SarifLog();
    }

    /// <summary>
    /// Maps SARIF logs to the code analysis model; the project name is taken from the file name. Logs with the same
    /// project name (e.g. one per target framework) are merged and identical results reported once.
    /// Suppressed results and results below <paramref name="minLevelRank"/> are skipped.
    /// </summary>
    internal static CodeAnalysis ToCodeAnalysis(IEnumerable<(string ProjectName, SarifLog Log)> logs, int minLevelRank)
    {
        var rules = new Dictionary<string, SarifRule>(StringComparer.Ordinal);
        var projects = new List<Project>();

        var projectLogs = logs
            .GroupBy(x => x.ProjectName, StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase);
        foreach (var group in projectLogs)
        {
            var diagnostics = new Dictionary<(string, string, string, int, int), ProjectDiagnostic>();
            foreach (var run in group.SelectMany(x => x.Log.Runs ?? []))
            {
                foreach (var rule in run.Tool?.Driver?.Rules ?? [])
                {
                    if (!string.IsNullOrEmpty(rule.Id))
                    {
                        rules.TryAdd(rule.Id, rule);
                    }
                }

                foreach (var result in run.Results ?? [])
                {
                    if (string.IsNullOrEmpty(result.RuleId) || IsSuppressed(result) || LevelRank(result) < minLevelRank)
                    {
                        continue;
                    }

                    var diagnostic = ToProjectDiagnostic(result.RuleId, result);
                    var key = (diagnostic.Id, diagnostic.Message, diagnostic.FilePath, diagnostic.Location.Line, diagnostic.Location.Character);
                    diagnostics.TryAdd(key, diagnostic);
                }
            }

            if (diagnostics.Count > 0)
            {
                projects.Add(new Project
                {
                    Name = group.Key,
                    Diagnostics = diagnostics.Values
                        .OrderBy(x => x.FilePath, StringComparer.Ordinal)
                        .ThenBy(x => x.Location.Line)
                        .ThenBy(x => x.Location.Character)
                        .ToArray(),
                });
            }
        }

        var summary = projects
            .SelectMany(x => x.Diagnostics)
            .GroupBy(x => x.Id, StringComparer.Ordinal)
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .Select(x =>
            {
                rules.TryGetValue(x.Key, out var rule);
                var title = Normalize(rule?.ShortDescription?.Text);
                return new Diagnostic
                {
                    Id = x.Key,
                    Count = x.Count(),
                    Title = string.IsNullOrEmpty(title) ? x.First().Message : title,
                    Description = Normalize(rule?.FullDescription?.Text),
                    HelpLink = rule?.HelpUri ?? string.Empty,
                };
            })
            .ToArray();

        return new CodeAnalysis { Summary = summary, Projects = projects.ToArray() };
    }

    private static ProjectDiagnostic ToProjectDiagnostic(string ruleId, SarifResult result)
    {
        var physical = result.Locations?.FirstOrDefault()?.PhysicalLocation;
        return new ProjectDiagnostic
        {
            Id = ruleId,
            Severity = result.Level ?? "warning",
            Message = Normalize(result.Message?.Text),
            FilePath = ToFilePath(physical?.ArtifactLocation?.Uri),
            Location = new Location
            {
                Line = physical?.Region?.StartLine ?? 0,
                Character = physical?.Region?.StartColumn ?? 0,
            },
        };
    }

    // SARIF 2.1.0 section 3.27.10: a missing level means "warning".
    private static int LevelRank(SarifResult result)
        => TryParseLevel(result.Level ?? "warning", out var rank) ? rank : 0;

    private static bool IsSuppressed(SarifResult result)
        => result.Suppressions?.Any(x => x.Status is null || x.Status == "accepted") == true;

    private static string ToFilePath(string? uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            return string.Empty;
        }

        var path = Uri.TryCreate(uri, UriKind.Absolute, out var u) && u.IsFile ? u.LocalPath : uri;
        return path.Replace('\\', '/');
    }

    private static string Normalize(string? text)
        => text is null ? string.Empty : text.Replace("\r\n", " ").Replace('\n', ' ').Trim();
}
