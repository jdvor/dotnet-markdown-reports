#pragma warning disable SA1402

namespace DotnetMarkdownReports.App.Cobertura;

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using static DotnetMarkdownReports.App.Constants;

[XmlRoot("coverage")]
public class Coverage
{
    [XmlAttribute("line-rate")]
    public float LineRate { get; set; }

    [XmlAttribute("branch-rate")]
    public float BranchRate { get; set; }

    [XmlAttribute("lines-covered")]
    public int LinesCovered { get; set; }

    [XmlAttribute("lines-valid")]
    public int LinesValid { get; set; }

    [XmlAttribute("branches-covered")]
    public int BranchesCovered { get; set; }

    [XmlAttribute("branches-valid")]
    public int BranchesValid { get; set; }

    [XmlAttribute("complexity")]
    public int Complexity { get; set; }

    [XmlAttribute("version")]
    public string Version { get; set; } = string.Empty;

    [XmlAttribute("timestamp")]
    public int Timestamp { get; set; }

    [XmlArray("packages")]
    [XmlArrayItem(ElementName = "package")]
    public Package[] Packages { get; set; } = Array.Empty<Package>();

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Cobertura.Coverage", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Cobertura.Package", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Cobertura.ClassInfo", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Cobertura.Method", AsmName)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, $"{RootNs}.Cobertura.Line", AsmName)]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "using DynamicDependency attributes")]
    internal static Coverage ReadFromFile(FileInfo file)
    {
        using var stream = file.OpenRead();
        using var reader = new XmlTextReader(stream);
        var serializer = new XmlSerializer(typeof(Coverage));
        return (Coverage)serializer.Deserialize(reader)!;
    }
}

public class Package
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("line-rate")]
    public float LineRate { get; set; }

    [XmlAttribute("branch-rate")]
    public float BranchRate { get; set; }

    [XmlAttribute("complexity")]
    public int Complexity { get; set; }

    [XmlArray("classes")]
    [XmlArrayItem(ElementName = "class")]
    public ClassInfo[] Classes { get; set; } = Array.Empty<ClassInfo>();
}

public class ClassInfo
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("filename")]
    public string FileName { get; set; } = string.Empty;

    [XmlAttribute("line-rate")]
    public float LineRate { get; set; }

    [XmlAttribute("branch-rate")]
    public float BranchRate { get; set; }

    [XmlAttribute("complexity")]
    public int Complexity { get; set; }

    [XmlArray("methods")]
    [XmlArrayItem(ElementName = "method")]
    public Method[] Methods { get; set; } = Array.Empty<Method>();
}

public class Method
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("signature")]
    public string Signature { get; set; } = string.Empty;

    [XmlAttribute("line-rate")]
    public float LineRate { get; set; }

    [XmlAttribute("branch-rate")]
    public float BranchRate { get; set; }

    [XmlAttribute("complexity")]
    public int Complexity { get; set; }

    [XmlArray("lines")]
    [XmlArrayItem(ElementName = "line")]
    public Line[] Lines { get; set; } = Array.Empty<Line>();
}

public class Line
{
    [XmlAttribute("number")]
    public int Number { get; set; }

    [XmlAttribute("hits")]
    public int Hits { get; set; }

    [XmlAttribute("branch")]
    public string Branch { get; set; } = "False"; // True | False
}

#pragma warning restore SA1402
