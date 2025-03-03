namespace VContainerSourceGenerator.Utils;

using Microsoft.CodeAnalysis;

public static class Logger
{
    public static SourceProductionContext Context { set; private get; }

    public static void Log(string message)
    {
        Context.ReportDiagnostic(Diagnostic.Create(
                  new DiagnosticDescriptor(
                      "SG0001",
                      "Source Generator Log",
                     message,
                      "Debug",
                      DiagnosticSeverity.Info,
                      isEnabledByDefault: true),
                  Location.None));
    }
}
