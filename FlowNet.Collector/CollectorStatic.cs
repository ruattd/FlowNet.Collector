using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FlowNet.Core;

namespace FlowNet.Collector;

/// <summary>
/// Collector static methods.
/// </summary>
public static class CollectorStatic
{
    private static readonly Flow.ExtensionContext Context = Flow.Internal.CreateExtension("flow:collector");

    /// <summary>
    /// Invoke a collection task.
    /// <b>Do NOT use it outside of collector source generators.</b>
    /// </summary>
    public static Task InvokeCollectionTask<TArgument>(
        string taskIdentifier, TArgument arg, [CallerMemberName] string caller = "")
    {
        if (caller != "InvokeComponentCollection") throw new InvalidOperationException();
        // Rider will suggest that there is a CS8714 warning, but this is a bug, the C# compiler does not report anything.
        // ReSharper disable once CS8714
        return Context.InvokeTask(taskIdentifier, arg, true);
    }
}
