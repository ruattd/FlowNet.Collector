# Flow.NET Collector

Flow.NET 扩展: 基于源生成器的组件收集器

## 有什么用

通过自定义的元注解 (Meta Attribute) 来标记任意 `internal` 或 `public` 的静态成员，自动生成引用该成员调用指定收集入口的代码。

## 如何使用

引入依赖:

```shell
dotnet add package FlowNet.Collector
```

你也可以直接在项目声明文件里写 `PackageReference`:

```xml
<ItemGroup>
    <PackageReference Include="FlowNet.Collector" Version="0.1.0"/>
</ItemGroup>
```

在 `AssemblyInfo.cs` 引用扩展:

```csharp
using FlowNet.Collector;

[assembly: UseCollector]
```

编写元注解:

```csharp
[CollectorMeta("Example.Target.CollectEntry")]
[AttributeUsage(AttributeTargets.All)]
public class TargetEntryAttribute : Attribute;
```

标记成员:

```csharp
namespace Example;

public class ExampleClass {
    [TargetEntry]
    public static TargetType Target { get; set; }
}
```

将在 `FlowInterops` 中生成以下调用代码:

```csharp
Example.Target.CollectEntry(Example.ExampleClass.Target);
```
