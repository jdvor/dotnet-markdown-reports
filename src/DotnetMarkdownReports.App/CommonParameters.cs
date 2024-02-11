namespace DotnetMarkdownReports.App;

using Cocona;
using System.Diagnostics.CodeAnalysis;

public record CommonParameters(

    [Argument(Description = Desc.SourcePathArg)] string SourcePath,

    [Option('o', Description = Desc.OutputFilePathOpt)]
    string OutputFilePath = "",

    [Option('q', Description = Desc.QuietOpt)]
    bool Quiet = false) : ICommandParameterSet;
