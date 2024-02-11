using Cocona;
using DotnetMarkdownReports.App;
using DotnetMarkdownReports.App.Cobertura;
using DotnetMarkdownReports.App.Roslynator;
using DotnetMarkdownReports.App.Trx;

var app = CoconaLiteApp.Create(args, options =>
{
    options.EnableConvertOptionNameToLowerCase = true;
    options.EnableConvertArgumentNameToLowerCase = true;
    options.EnableConvertCommandNameToLowerCase = true;
    options.ShutdownTimeout = TimeSpan.FromSeconds(5);
});
app.AddCommand("trx", (CommonParameters common, [Option('s', Description = Desc.OnlySummaryOpt)] bool onlySummary) =>
    {
        var cmd = new TrxBasicMarkdown(common);
        return cmd.Execute();
    })
    .WithDescription(Desc.TrxCmd);

app.AddCommand("cobertura", (CommonParameters common) =>
    {
        var cmd = new CoberturaBasicMarkdown(common);
        return cmd.Execute();
    })
    .WithDescription(Desc.CoberturaCmd);

app.AddCommand(
        "roslynator",
        (CommonParameters common, [Option('s', Description = Desc.OnlySummaryOpt)] bool onlySummary) =>
    {
        var cmd = new RoslynatorBasicMarkdown(common);
        return cmd.Execute();
    })
    .WithDescription(Desc.RoslynatorCmd);

app.Run();
