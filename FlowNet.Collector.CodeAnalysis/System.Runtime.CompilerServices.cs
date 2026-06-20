using System.ComponentModel;

namespace System.Runtime.CompilerServices;

// This file is to add support for 'required' modifier and 'init' setter in CLR properties, because
// some targets (e.g. .NET Standard 2.0) don't have those attributes to support the features mentioned above.
// If you don't need these features, you can remove this file safely.

#if !NET5_0_OR_GREATER

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit {}

#endif // !NET5_0_OR_GREATER

#if !NET7_0_OR_GREATER

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
{
    public string FeatureName { get; } = featureName;
    public bool IsOptional { get; init; }

    public const string RefStructs = nameof(RefStructs);
    public const string RequiredMembers = nameof(RequiredMembers);
}

#endif // !NET7_0_OR_GREATER
