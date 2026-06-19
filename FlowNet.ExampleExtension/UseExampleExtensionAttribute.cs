using System;
using FlowNet.Core;

namespace FlowNet.ExampleExtension;

/// <summary>
/// Add this attribute to assembly info to load Flow.NET Example Extension.
/// </summary>
[FlowExtensionUsage("Init_FlowNet_ExampleExtension")]
public sealed class UseExampleExtensionAttribute : Attribute;
