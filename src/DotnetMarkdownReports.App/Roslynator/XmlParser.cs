#pragma warning disable SA1402

namespace DotnetMarkdownReports.App.Roslynator;

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using static DotnetMarkdownReports.App.Constants;

[XmlRoot("Roslynator")]
public class RoslynatorReport
{
    public CodeAnalysis CodeAnalysis { get; set; } = new();

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Roslynator.RoslynatorReport", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Roslynator.CodeAnalysis", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Roslynator.Diagnostic", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Roslynator.Project", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Roslynator.ProjectDiagnostic", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Roslynator.Location", AsmName)]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "using DynamicDependency attributes")]
    internal static RoslynatorReport ReadFromFile(FileInfo file)
    {
        using var stream = file.OpenRead();
        using var reader = new XmlTextReader(stream);
        var serializer = new XmlSerializer(typeof(RoslynatorReport));
        return (RoslynatorReport)serializer.Deserialize(reader)!;
    }
}

public class CodeAnalysis
{
    [XmlArray]
    [XmlArrayItem(ElementName = "Diagnostic")]
    public Diagnostic[] Summary { get; set; } = Array.Empty<Diagnostic>();

    [XmlArray]
    [XmlArrayItem(ElementName = "Project")]
    public Project[] Projects { get; set; } = Array.Empty<Project>();
}

public class Diagnostic
{
    [XmlAttribute("Id")]
    public string Id { get; set; } = string.Empty;

    [XmlAttribute("Title")]
    public string Title { get; set; } = string.Empty;

    [XmlAttribute("Count")]
    public int Count { get; set; }

    public string Description { get; set; } = string.Empty;

    public string HelpLink { get; set; } = string.Empty;
}

public class Project
{
    [XmlAttribute("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("FilePath")]
    public string FilePath { get; set; } = string.Empty;

    [XmlArray]
    [XmlArrayItem(ElementName = "Diagnostic")]
    public ProjectDiagnostic[] Diagnostics { get; set; } = Array.Empty<ProjectDiagnostic>();
}

public class ProjectDiagnostic
{
    [XmlAttribute("Id")]
    public string Id { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public Location Location { get; set; } = new();
}

public class Location
{
    [XmlAttribute("Line")]
    public int Line { get; set; }

    [XmlAttribute("Character")]
    public int Character { get; set; }
}

#pragma warning restore SA1402
