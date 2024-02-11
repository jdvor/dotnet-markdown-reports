namespace DotnetMarkdownReports.App;

using System.Text;

internal sealed class MarkdownTable
{
    private readonly string[] headers;
    private readonly int[] columnWidths;
    private readonly List<string[]> rows;

    public MarkdownTable(string[] headers, int estimatedRows = 0)
    {
        this.headers = headers;
        rows = estimatedRows > 0
            ? new List<string[]>(estimatedRows)
            : new List<string[]>();
        columnWidths = headers.Select(x => x.Length).ToArray();
    }

    public MarkdownTable AddRow(params string[] values)
    {
        var row = new string[headers.Length];
        for (var i = 0; i < values.Length; i++)
        {
            if (i >= row.Length)
            {
                break;
            }

            var value = values[i];
            if (columnWidths[i] < value.Length)
            {
                columnWidths[i] = value.Length;
            }

            row[i] = value;
        }

        rows.Add(row);

        return this;
    }

    public void WriteTo(StringBuilder sb)
    {
        if (rows.Count == 0)
        {
            return;
        }

        const string lead = "| ";
        const string trail = " |";
        const string sep = " | ";

        // header
        for (var i = 0; i < headers.Length; i++)
        {
            sb.Append(i == 0 ? lead : sep);
            var width = columnWidths[i];
            var value = headers[i].PadRight(width);
            sb.Append(value);
        }

        sb.AppendLine(trail);

        // row separator between header and data
        sb.Append(lead);
        sb.Append(string.Join(sep, columnWidths.Select(RepeatDash)));
        sb.AppendLine(trail);

        // data
        foreach (var row in rows)
        {
            for (var i = 0; i < row.Length; i++)
            {
                sb.Append(i == 0 ? lead : sep);
                var width = columnWidths[i];
                var value = row[i].PadRight(width);
                sb.Append(value);
            }

            sb.AppendLine(trail);
        }
    }

    private static string RepeatDash(int n)
        => new(Enumerable.Range(0, n).Select(_ => '-').ToArray());
}
