using System.Collections.Immutable;
using C2CS;
using C2CS.Options;
using JetBrains.Annotations;

namespace Flecs.Bindgen;

[UsedImplicitly]
public class ReaderCCode : IReaderCCode
{
    public ReaderCCodeOptions Options { get; } = new();

    public ReaderCCode()
    {
        Configure(Options);
    }

    private static void Configure(ReaderCCodeOptions options)
    {
        options.InputHeaderFilePath =
            "../src/c/production/flecs/include/flecs_pinvoke.h";
        options.UserIncludeDirectories = new[] { "../ext/flecs/include" }.ToImmutableArray();

        if (Native.OperatingSystem == NativeOperatingSystem.macOS)
        {
            var platform = new Dictionary<TargetPlatform, ReaderCCodeOptionsPlatform>();
       
            platform.Add(TargetPlatform.aarch64_apple_darwin, new ReaderCCodeOptionsPlatform());
            // platform.Add(TargetPlatform.x86_64_apple_darwin, new ReaderCCodeOptionsPlatform());
            options.Platforms = platform.ToImmutableDictionary();
        }

        options.OutputAbstractSyntaxTreesFileDirectory =
            "./ast";
    }
}