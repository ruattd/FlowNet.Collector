using System.Text;
using FlowNet.ExampleExtension.CodeAnalysis.Shared;
using Microsoft.CodeAnalysis;

namespace FlowNet.ExampleExtension.CodeAnalysis.SourceGenerators;

[Generator]
public class ExtensionInitGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateExtensionInit);
    }

    private static void GenerateExtensionInit(IncrementalGeneratorPostInitializationContext context)
    {
        var sb = new StringBuilder();
        sb.AppendCommonHeader();
        sb.AppendLine();
        sb.AppendLine("namespace FlowNet.Core;");
        sb.AppendLine();
        sb.AppendLine("partial class FlowInterops");
        sb.AppendLine("{");
        sb.Append("    ").AppendGeneratedCodeAttribute();
        sb.Append("    ").AppendExcludeFromCodeCoverageAttribute();
        sb.AppendLine("    private static async Task Init_FlowNet_ExampleExtension()");
        sb.AppendLine("    {");
        sb.AppendLine("        // ... add your code here");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        context.AddSource("FlowNet.ExampleExtension.Init.g.cs", sb.ToString());
    }
}
