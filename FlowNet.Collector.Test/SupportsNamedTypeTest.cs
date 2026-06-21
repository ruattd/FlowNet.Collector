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
[CollectorMeta.SupportsNamedType(TypeGenerationMode.EmptyConstructor, true)]
[CollectorMeta.SupportsMethod<Func<ISupportsNamedTypeTest>>]
[AttributeUsage(AttributeTargets.Class)]
public sealed class TargetClassEmptyCtorDelegateAttribute : Attribute;

[CollectorMeta("FlowNet.Collector.Test.SupportsNamedTypeTestClass.Collect")]
[CollectorMeta.SupportsNamedType(TypeGenerationMode.TypeofKeyword)]
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class TargetTypeAttribute : Attribute;

public interface ISupportsNamedTypeTest;

[TargetClass("test")]
[TargetClassEmptyCtor("test")]
[TargetType]
[TargetClassEmptyCtorDelegate]
public class SupportsNamedTypeTestClass(string test = "") : ISupportsNamedTypeTest
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

    public static Task Collect(Func<ISupportsNamedTypeTest> target)
    {
        var t = target();
        Console.WriteLine("Type: " + t.GetType().FullName);
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
