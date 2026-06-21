using System;
using System.Threading.Tasks;
using FlowNet.Core;

namespace FlowNet.Collector.Test;

[CollectorMeta("FlowNet.Collector.Test.SupportsNamedTypeTestClass.Collect")]
[CollectorMeta.SupportsNamedType(TypeGenerationMode.Constructor)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class TargetClassAttribute(string test) : Attribute;

[CollectorMeta("FlowNet.Collector.Test.SupportsNamedTypeTestClass.Collect")]
[CollectorMeta.SupportsNamedType(TypeGenerationMode.EmptyConstructor)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class TargetClassEmptyCtorAttribute(string test) : Attribute;

[CollectorMeta("FlowNet.Collector.Test.SupportsNamedTypeTestClass.Collect")]
[CollectorMeta.SupportsNamedType(TypeGenerationMode.TypeofKeyword)]
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class TargetTypeAttribute : Attribute;

[TargetClass("test")]
[TargetClassEmptyCtor("test")]
[TargetType]
public class SupportsNamedTypeTestClass(string test = "")
{
    public static Task Collect(Type target, string test = "")
    {
        Console.WriteLine($"Test arg: {test}");
        return Task.CompletedTask;
    }

    public static Task Collect(SupportsNamedTypeTestClass? target, string test = "")
    {
        Console.WriteLine($"Test arg: {test}");
        return Task.CompletedTask;
    }
}

[TestClass]
public class SupportsNamedTypeTest
{
    [TestMethod]
    public async Task Test()
    {
        await FlowInterops.Initialize();
    }
}
