using System.Collections.Immutable;
using C2CS;
using C2CS.Options;
using JetBrains.Annotations;

namespace Flecs.Bindgen;

[UsedImplicitly]
public class WriterCSharpCode : IWriterCSharpCode
{
    public WriterCSharpCodeOptions Options { get; } = new();

    public WriterCSharpCode()
    {
        Configure(Options);
    }

    private static void Configure(WriterCSharpCodeOptions options)
    {
        options.InputAbstractSyntaxTreesFileDirectory = "./ast";
        
        options.OutputCSharpCodeFilePath = "../src/cs/production/Flecs/flecs.cs";
        options.NamespaceName = "flecs_hub";
        options.LibraryName = "flecs";
        options.IgnoredNames = new[]
        {
            "FLECS_FLOAT",
            "ECS_FUNC_NAME_BACK"
        }.ToImmutableArray()!;
    }
}