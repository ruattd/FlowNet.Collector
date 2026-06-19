using System.Text;
using FlowNet.Collector.CodeAnalysis.Shared;
using Microsoft.CodeAnalysis;

namespace FlowNet.Collector.CodeAnalysis.SourceGenerators;

[Generator]
public class ExtensionInitGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateComponentCollectionMethod);
    }

    private static void GenerateComponentCollectionMethod(IncrementalGeneratorPostInitializationContext context)
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
        sb.AppendLine("    private static async Task InvokeComponentCollection()");
        sb.AppendLine("    {");
        // TODO
        sb.AppendLine("    }");
        sb.AppendLine("}");
        context.AddSource("FlowInterops.InvokeComponentCollection.g.cs", sb.ToString());
    }
}
