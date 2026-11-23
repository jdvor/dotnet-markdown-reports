#pragma warning disable SA1402

namespace DotnetMarkdownReports.App.Sarif;

using System.Text.Json.Serialization;

// Minimal subset of SARIF 2.1.0 needed for the code analysis report.
internal sealed class SarifLog
{
    public List<SarifRun>? Runs { get; init; }
}

internal sealed class SarifRun
{
    public SarifTool? Tool { get; init; }

    public List<SarifResult>? Results { get; init; }
}

internal sealed class SarifTool
{
    public SarifDriver? Driver { get; init; }
}

internal sealed class SarifDriver
{
    public List<SarifRule>? Rules { get; init; }
}

internal sealed class SarifRule
{
    public string? Id { get; init; }

    public SarifMessage? ShortDescription { get; init; }

    public SarifMessage? FullDescription { get; init; }

    public string? HelpUri { get; init; }
}

internal sealed class SarifMessage
{
    public string? Text { get; init; }
}

internal sealed class SarifResult
{
    public string? RuleId { get; init; }

    public string? Level { get; init; }

    public SarifMessage? Message { get; init; }

    public List<SarifLocation>? Locations { get; init; }

    public List<SarifSuppression>? Suppressions { get; init; }
}

internal sealed class SarifLocation
{
    public SarifPhysicalLocation? PhysicalLocation { get; init; }
}

internal sealed class SarifPhysicalLocation
{
    public SarifArtifactLocation? ArtifactLocation { get; init; }

    public SarifRegion? Region { get; init; }
}

internal sealed class SarifArtifactLocation
{
    public string? Uri { get; init; }
}

internal sealed class SarifRegion
{
    public int StartLine { get; init; }

    public int StartColumn { get; init; }
}

internal sealed class SarifSuppression
{
    public string? Kind { get; init; }

    public string? Status { get; init; }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(SarifLog))]
internal sealed partial class SarifJsonContext : JsonSerializerContext
{
}

#pragma warning restore SA1402
