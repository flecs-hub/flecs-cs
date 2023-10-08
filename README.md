# flecs-cs

Archived; see: https://github.com/BeanCheeseBurrito/Flecs.NET

Automatically updated C# bindings for https://github.com/SanderMertens/flecs with native dynamic link libraries. Includes the lower-level unsafe C# "binding" which is 100% automatically generated. The higher level safe "wrapper" is still being worked on.

## How to use

### NuGet

NuGet packages are experimental; please add the NuGet feed `https://www.myget.org/F/bottlenoselabs/api/v3/index.json` to access the NuGet packages.

Either install `flecs_hub.Flecs` or `flecs_hub.Interop.Flecs` along with one the native library packages for your operating system / hardware.

|Package name|Description|
|-|-|
|`flecs_hub.Flecs`|High-level, safe, idomatic C# API.|
|`flecs_hub.Interop.Flecs`|Low-level, unsafe, 1-1 C# bindings.|
|`flecs_hub.Interop.Flecs.runtime.win-x64`|Native libraries for Windows x64.|
|`flecs_hub.Interop.Flecs.runtime.osx`|Native libraries for macOS universal (x64 and arm64).|
|`flecs_hub.Interop.Flecs.runtime.linux-x64`|Native libraries for Linux x64.|

### From source

1. Download and install [.NET 7](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/flecs-hub/flecs-cs`.
3. Build the native library by running `library.sh`. To execute `.sh` scripts on Windows, use Git Bash which can be installed with Git itself: https://git-scm.com/download/win. The `library.sh` script requires that CMake is installed and in your path and that [`C2CS`](https://github.com/bottlenoselabs/c2cs) is installed.
4. 
   - .NET 7+: Add the `src/cs/production/Flecs.Core/Flecs.Core.csproj` C# project to your solution as an existing project and then reference it within your own solution.
   - Unity: Build the `src/cs/production/Flecs.Unity/Flecs.Unity.csproj` C# project and create the resulting `Flecs.Unity.dll` with it's dependencies; follow Unity's documentation for using the compiled C# code as a `.dll`: https://docs.unity3d.com/Manual/UsingDLL.

## Developers: Documentation

### C# Examples

For examples in C# (.NET Core, not Unity), see [./src/cs/examples](https://github.com/flecs-hub/flecs-cs/tree/main/src/cs/examples), or open up the solution `.sln` file in VisualStudio / Rider.

### C Examples

To learn how to use `flecs` directly, check out the https://github.com/SanderMertens/flecs#documentation.

### Binding / wrapper

For more information on how C# binding / wrapper work, see [`C2CS`](https://github.com/lithiumtoast/c2cs), the tool that generates the binding / wrapper for `flecs` and other C libraries.

## License

`flecs-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`flecs` itself is licensed under MIT (`MIT`) - see https://github.com/SanderMertens/flecs/blob/master/LICENSE for more details.
