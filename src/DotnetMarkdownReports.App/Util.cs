namespace DotnetMarkdownReports.App;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.Text.RegularExpressions;

internal static class Util
{
    internal static bool TryGetSourceFiles(string sourcePath, string filePatternOrExtension, out List<FileInfo> files)
    {
        files = new List<FileInfo>();
        if (sourcePath.Contains('*'))
        {
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude(sourcePath);
            var cwd = new DirectoryInfo(Environment.CurrentDirectory);
            var matched = matcher.Execute(new DirectoryInfoWrapper(cwd));
            if (matched.HasMatches)
            {
                files.AddRange(matched.Files.Select(x => new FileInfo(x.Path)));
            }
        }
        else if (File.Exists(sourcePath))
        {
            files.Add(new FileInfo(sourcePath));
        }
        else if (Directory.Exists(sourcePath))
        {
            var dir = new DirectoryInfo(sourcePath);
            var searchPattern = filePatternOrExtension.Contains('*') || filePatternOrExtension.Contains('.')
                ? filePatternOrExtension
                : $"*.{filePatternOrExtension}";
            files.AddRange(dir.EnumerateFiles(searchPattern, SearchOption.AllDirectories));
        }

        return files.Count > 0;
    }

    internal static Regex CreateFilePathRegex(string x)
    {
        var pattern = $" in (?<Suffix>\\.cs):line (?<Line>\\d+)";
        return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    internal static bool TryParseRepoUrl(string? stacktrace, string repoUrlPrefix, Regex filePathRegex, out string url)
    {
        if (string.IsNullOrEmpty(stacktrace))
        {
            url = string.Empty;
            return false;
        }

        var m = filePathRegex.Match(stacktrace);
        if (!m.Success)
        {
            url = string.Empty;
            return false;
        }

        var path = m.Groups["Path"].Value;
        var line = m.Groups["Line"].Value;
        if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(line))
        {
            url = string.Empty;
            return false;
        }

        url = $"{repoUrlPrefix}/{path}#L{line}";
        return true;
    }
}
