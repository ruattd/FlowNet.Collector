using System;
using FlowNet.Core;

namespace FlowNet.Collector;

/// <summary>
/// Add this attribute to assembly info to load Flow.NET Collector extension.
/// </summary>
[FlowExtensionUsage("InvokeComponentCollection")]
public sealed class UseCollectorAttribute : Attribute;
