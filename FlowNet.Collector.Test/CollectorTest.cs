using System;
using System.Threading.Tasks;
using FlowNet.Core;

namespace FlowNet.Collector.Test;

public delegate void TestAction<T, T2>(T t, T2 t2);

[CollectorMeta("test:collect")]
[CollectorMeta.SupportsMethod<Action>]
[AttributeUsage(AttributeTargets.Method)]
public sealed class TaskTargetAttribute(string test) : Attribute;

[CollectorMeta("FlowNet.Collector.Test.CollectorTest.Collect")]
[CollectorMeta.SupportsMethod<Action>]
[AttributeUsage(AttributeTargets.Method)]
public sealed class TargetAttribute : Attribute;

[Flow.Scope("test")]
public static partial class CollectorTest
{
    [Flow.Task]
    public static Task Collect(Action target, string test = "")
    {
        target();
        Console.WriteLine($"Test arg: {test}");
        return Task.CompletedTask;
    }

    [Target]
    [TaskTarget("123")]
    public static void Target()
    {
        Console.WriteLine("Test test teSt TEsT");
    }
}
