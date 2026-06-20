using System;

namespace FlowNet.Collector;

#pragma warning disable CS9113 // Parameter is unread.

/// <summary>
/// Mark an attribute as a collector meta attribute.
/// </summary>
/// <param name="collectionPoint">Full name of collection method or other support types, see <see cref="CollectionPointType"/></param>
/// <param name="type">Collection point type, default to autodetect from collection point</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class CollectorMeta(
    string collectionPoint,
    CollectionPointType type = default
) : Attribute
{
    /// <summary>
    /// Mark the attribute as a method collector and specify the delegate type.
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type of target methods</typeparam>
    public sealed class SupportsMethod<TDelegate> : Attribute
        where TDelegate : Delegate;
}

#pragma warning restore CS9113 // Parameter is unread.
