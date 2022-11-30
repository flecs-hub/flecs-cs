using JetBrains.Annotations;
using C2CS;

namespace Flecs.Bindgen.Runner;

[UsedImplicitly]
internal class Program
{
    private static int Main(string[] args)
    {
        var rootFileDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../.."));
        var bindgenFileDirectory = Path.Combine(rootFileDirectory, "bindgen");
        var buildPluginCommand = $"dotnet build \"{rootFileDirectory}/src/cs/production/Flecs.Bindgen/Flecs.Bindgen.csproj\" -p:OutputPath=\"{bindgenFileDirectory}/plugins/Flecs.Bindgen\"";
        var shellOutput = buildPluginCommand.ExecuteShell();
        if (shellOutput.ExitCode != 0)
        {
            Console.WriteLine("Error building Flecs.Bindgen plugin C# project.");
            return shellOutput.ExitCode;
        }
        
        return 0;
    }
}