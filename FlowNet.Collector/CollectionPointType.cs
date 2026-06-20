namespace FlowNet.Collector;

/// <summary>
/// Represents available types of collection points.<br/>
/// A collection point means an invokable component to transfer components collected by collector meta attributes.
/// </summary>
public enum CollectionPointType
{
    /// <summary>
    /// Detect automatically from collection point name.
    /// </summary>
    AutoDetect = 0,

    /// <summary>
    /// Use a public static method as the collection point.
    /// </summary>
    Method = 1,

    /// <summary>
    /// Use a <see cref="FlowNet.Core.IFlowTask">task</see> as the collection point, you need to specify
    /// global identifier of the task as collection point in <see cref="CollectorMeta"/> attribute.
    /// </summary>
    FlowTask = 2,
}
