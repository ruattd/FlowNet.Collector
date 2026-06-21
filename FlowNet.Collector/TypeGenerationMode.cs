namespace FlowNet.Collector;

/// <summary>
/// Defines the modes used for generating named type (class, interface...) references.
/// </summary>
public enum TypeGenerationMode
{
    /// <summary>
    /// Use <c>typeof(T)</c> keyword to pass the type reference directly.
    /// </summary>
    TypeofKeyword = 0,

    /// <summary>
    /// Use <c>new T()</c> to pass an instance of the type.
    /// </summary>
    EmptyConstructor = 1,

    /// <summary>
    /// Use <c>new T(args)</c> to pass an instance of the type, with arguments from collector meta attribute.<br/>
    /// <b>NOTE</b>: This mode will prevent arguments from being passed to the collection point.
    /// </summary>
    Constructor = 2,
}
