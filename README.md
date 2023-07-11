# flecs-cs

Automatically updated C# bindings for https://github.com/SanderMertens/flecs with native dynamic link libraries. Includes the lower-level unsafe C# "binding" which is 100% automatically generated. The higher level safe "wrapper" is still being worked on.

## How to use

### From source

1. Download and install [.NET 7](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/flecs-hub/flecs-cs`.
3. Build the native library by running `library.sh`. To execute `.sh` scripts on Windows, use Git Bash which can be installed with Git itself: https://git-scm.com/download/win. The `library.sh` script requires that CMake is installed and in your path and that [`C2CS`](https://github.com/bottlenoselabs/c2cs) is installed.
4. Add the `src/cs/production/Flecs/Flecs.csproj` C# project to your solution as an existing project and thenreference it within your own solution.

## Developers: Documentation

### C# Examples

For examples in C#, see [./src/cs/examples](https://github.com/flecs-hub/flecs-cs/tree/main/src/cs/examples), or open up the solution `.sln` file in VisualStudio / Rider.

### C Examples

To learn how to use `flecs` directly, check out the https://github.com/SanderMertens/flecs#documentation.

### Binding / wrapper

For more information on how C# binding / wrapper work, see [`C2CS`](https://github.com/lithiumtoast/c2cs), the tool that generates the binding / wrapper for `flecs` and other C libraries.

## License

`flecs-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`flecs` itself is licensed under MIT (`MIT`) - see https://github.com/SanderMertens/flecs/blob/master/LICENSE for more details.
