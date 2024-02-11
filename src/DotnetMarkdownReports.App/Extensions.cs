namespace DotnetMarkdownReports.App;

using System.Text;
using System.Text.RegularExpressions;

internal static class Extensions
{
    internal static StringBuilder H1(this StringBuilder sb, string value)
    {
        sb.AppendLine($"# {value}");
        sb.AppendLine();
        return sb;
    }

    internal static StringBuilder H2(this StringBuilder sb, string value)
    {
        sb.AppendLine();
        sb.AppendLine($"## {value}");
        sb.AppendLine();
        return sb;
    }

    internal static StringBuilder H3(this StringBuilder sb, string value)
    {
        sb.AppendLine();
        sb.AppendLine($"### {value}");
        sb.AppendLine();
        return sb;
    }

    internal static StringBuilder HorizontalRule(this StringBuilder sb)
    {
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        return sb;
    }

    internal static StringBuilder Table(this StringBuilder sb, MarkdownTable table)
    {
        sb.AppendLine();
        table.WriteTo(sb);
        sb.AppendLine();
        return sb;
    }

    internal static StringBuilder Details(
        this StringBuilder sb,
        bool open,
        string summary,
        Action<StringBuilder> content)
    {
        var op = open ? " open" : string.Empty;
        sb.AppendLine($"<details{op}>");
        sb.AppendLine($"<summary>{summary}</summary>");
        content(sb);
        sb.AppendLine("</details>");
        sb.AppendLine();
        return sb;
    }

    internal static string EscapeMarkdownTableCell(this string? s)
    {
        return s is null
            ? string.Empty
            : s.Replace(Environment.NewLine, " ").Replace("|", "&#124;");
    }

    internal static string LeftTrimSourceCodePath(this string? path, string before = "src/")
    {
        if (path is null)
        {
            return string.Empty;
        }

        var idx = path.IndexOf(before, StringComparison.OrdinalIgnoreCase);
        return idx <= 0
            ? path
            : path[idx..];
    }

    internal static string PadLeftNonBreakingSpace(this string? s, int length)
    {
        const string pad = "&nbsp;";
        var times = string.IsNullOrEmpty(s)
            ? length
            : length - s.Length;
        if (times <= 0)
        {
            return s ?? string.Empty;
        }

        return string.Join(string.Empty, Enumerable.Range(0, times).Select(_ => pad).Append(s));
    }
}
