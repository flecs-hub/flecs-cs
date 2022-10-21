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
        options.OutputAbstractSyntaxTreesFileDirectory =
            "./ast";

        var platforms = new Dictionary<TargetPlatform, ReaderCCodeOptionsPlatform>();
        if (Native.OperatingSystem == NativeOperatingSystem.macOS)
        {
            platforms.Add(TargetPlatform.aarch64_apple_darwin, new ReaderCCodeOptionsPlatform());
            platforms.Add(TargetPlatform.x86_64_apple_darwin, new ReaderCCodeOptionsPlatform());
        }
        else if (Native.OperatingSystem == NativeOperatingSystem.Windows)
        {
            platforms.Add(TargetPlatform.aarch64_pc_windows_msvc, new ReaderCCodeOptionsPlatform());
            platforms.Add(TargetPlatform.x86_64_pc_windows_msvc, new ReaderCCodeOptionsPlatform());
        }
        else if (Native.OperatingSystem == NativeOperatingSystem.Linux)
        {
            platforms.Add(TargetPlatform.aarch64_unknown_linux_gnu, new ReaderCCodeOptionsPlatform());
            platforms.Add(TargetPlatform.x86_64_unknown_linux_gnu, new ReaderCCodeOptionsPlatform());
        }
        
        options.Platforms = platforms.ToImmutableDictionary();
    }
}