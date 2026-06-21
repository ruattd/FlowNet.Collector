using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using FlowNet.Collector.CodeAnalysis.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FlowNet.Collector.CodeAnalysis.SourceGenerators;

[Generator]
public class ComponentCollectionGenerator : IIncrementalGenerator
{
    private const int CPType_Method = 1;
    private const int CPType_FlowTask = 2;

    private const int TGMode_TypeofKeyword = 0;
    private const int TGMode_EmptyConstructor = 1;
    private const int TGMode_Constructor = 2;

    private record CollectorMetaInfo(
        string CollectionPoint,
        int CollectionPointType,
        string? TargetMethodDelegateQualifiedName,
        int? NamedTypeGenerationMode,
        bool NamedTypeUseDelegate
    );

    private readonly record struct TargetComponentInfo(
        string FullQualifiedName,
        SymbolKind Kind,
        CollectorMetaInfo Collector,
        ImmutableArray<TypedConstant> Arguments,
        ImmutableArray<ITypeSymbol> TypeArguments
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // collect all symbols with meta attributes
        var targets = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (_, _) => true,
            transform: static (ctx, _) =>
            {
                var symbol = ctx.SemanticModel.GetDeclaredSymbol(ctx.Node);
                if (symbol?.GetAttributes() is not { Length: > 0 } attrs) return default;
                return (symbol, attrs);
            })
            .Where(static x => x != default)
            .SelectMany(SelectTargetComponents)
            .Collect();

        // generate source file
        context.RegisterSourceOutput(targets, GenerateComponentCollectionMethod);
    }

    private static IEnumerable<TargetComponentInfo> SelectTargetComponents(
        (ISymbol, ImmutableArray<AttributeData>) pair, CancellationToken cancelToken)
    {
        var (symbol, attrs) = pair;
        var collectors = new Dictionary<INamedTypeSymbol, CollectorMetaInfo?>(SymbolEqualityComparer.Default);
        string? symbolName = null;
        foreach (var attr in attrs)
        {
            if (attr.AttributeClass == null) continue;
            // get collector info or extract new
            if (!collectors.TryGetValue(attr.AttributeClass, out var collector))
            {
                AttributeData? metaMarkupAttr = null, supportsMethodAttr = null, supportsNamedTypeAttr = null;
                foreach (var a in attr.AttributeClass.GetAttributes())
                {
                    var name = a.AttributeClass?.GetSimplifiedTypeName();
                    if (name == Constants.CollectorMetaAttribute) metaMarkupAttr = a;
                    if (name == Constants.SupportsMethodAttribute) supportsMethodAttr = a;
                    if (name == Constants.SupportsNamedTypeAttribute) supportsNamedTypeAttr = a;
                }
                // mark as not a collector first
                collector = null;
                // try extract collector info
                if (metaMarkupAttr?.ConstructorArguments[0].Value is string cp)
                {
                    // cp type, default to detect automatically from cp
                    var type = metaMarkupAttr.ConstructorArguments[1].Value as int?;
                    if (type is null or 0) type = cp.Contains(':') ? CPType_FlowTask : CPType_Method;
                    // target method delegate type name
                    var targetMethodDelegateQualifiedName = supportsMethodAttr?.AttributeClass?.TypeArguments[0].GetFullyQualifiedName();
                    var namedTypeGenerationMode = supportsNamedTypeAttr?.ConstructorArguments[0].Value as int?;
                    var namedTypeUseDelegate = supportsNamedTypeAttr?.ConstructorArguments[1].Value as bool? ?? false;
                    collector = new CollectorMetaInfo(cp, type.Value, targetMethodDelegateQualifiedName, namedTypeGenerationMode, namedTypeUseDelegate);
                }
                collectors[attr.AttributeClass] = collector;
            }
            // collector is null means this attribute isn't a collector, skip
            if (collector == null) continue;
            // extract args from attr
            var args = attr.ConstructorArguments;
            var typeArgs = attr.AttributeClass.TypeArguments;
            // yield target info
            yield return new TargetComponentInfo(symbolName ??= symbol.GetQualifiedSymbolName(), symbol.Kind, collector, args, typeArgs);
        }
    }

    private static void GenerateComponentCollectionMethod(
        SourceProductionContext context,
        ImmutableArray<TargetComponentInfo> targets)
    {
        var sb = new StringBuilder();
        sb.AppendCommonHeader();
        sb.AppendLine();
        sb.AppendLine("namespace FlowNet.Core;");
        sb.AppendLine();
        sb.AppendLine("partial class FlowInterops");
        sb.AppendLine("{");
        sb.Append("    ").AppendGeneratedCodeAttribute().Append("    ").AppendExcludeFromCodeCoverageAttribute();
        sb.AppendLine("    private static async Task InvokeComponentCollection()");
        sb.AppendLine("    {");
        foreach (var target in targets)
        {
            var collector = target.Collector;
            sb.Append("        await ");
            if (target.Kind == SymbolKind.Method && collector.TargetMethodDelegateQualifiedName == null) continue;
            if (target.Kind == SymbolKind.NamedType && collector.NamedTypeGenerationMode == null) continue;
            void AppendArguments(bool appendComma = true)
            {
                var it = target.Arguments.GetEnumerator();
                if (!it.MoveNext()) return;
                if (appendComma) sb.Append(", ");
                sb.Append(it.Current.ToCSharpString());
                while (it.MoveNext()) sb.Append(", ").Append(it.Current.ToCSharpString());
            }
            void AppendSymbolReferenceCode()
            {
                var qualifiedName = target.FullQualifiedName;
                var appendArguments = true;
                if (target.Kind == SymbolKind.Method)
                {
                    sb.Append("new ").Append(collector.TargetMethodDelegateQualifiedName).Append("(").Append(qualifiedName).Append(")");
                }
                else if (target.Kind == SymbolKind.NamedType)
                {
                    switch (collector.NamedTypeGenerationMode)
                    {
                        case TGMode_TypeofKeyword:
                            sb.Append("typeof(").Append(qualifiedName).Append(")");
                            break;
                        case TGMode_EmptyConstructor or TGMode_Constructor:
                            if (collector.NamedTypeUseDelegate)
                            {
                                var delegateType = collector.TargetMethodDelegateQualifiedName ?? "System.Func<object>";
                                sb.Append("new ").Append(delegateType).Append("(() => ");
                            }
                            sb.Append("new ").Append(qualifiedName).Append('(');
                            if (collector.NamedTypeGenerationMode == TGMode_Constructor)
                            {
                                AppendArguments(false);
                                appendArguments = false;
                            }
                            sb.Append(')');
                            if (collector.NamedTypeUseDelegate) sb.Append(')');
                            break;
                    }
                }
                else sb.Append(qualifiedName);
                if (appendArguments) AppendArguments();
            }
            switch (collector.CollectionPointType)
            {
                case CPType_Method:
                    sb.Append(collector.CollectionPoint);
                    if (target.TypeArguments.Length > 0)
                        sb.Append("<").Append(string.Join(", ", target.TypeArguments.Select(t => t.GetQualifiedSymbolName()))).Append(">");
                    sb.Append("(");
                    AppendSymbolReferenceCode();
                    break;
                case CPType_FlowTask:
                    if (target.TypeArguments.Length > 0) sb.AppendLine("        // WARNING: Flow task with type arguments is not supported");
                    sb.Append("CollectorStatic.InvokeCollectionTask(\"").Append(collector.CollectionPoint).Append("\", ");
                    if (target.Arguments.Length > 0) sb.Append("(");
                    AppendSymbolReferenceCode();
                    if (target.Arguments.Length > 0) sb.Append(")");
                    break;
            }
            sb.AppendLine(").ConfigureAwait(false);");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        context.AddSource("FlowInterops.InvokeComponentCollection.g.cs", sb.ToString());
    }
}
