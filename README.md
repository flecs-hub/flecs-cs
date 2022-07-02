# flecs-cs

Automatically updated C# bindings for https://github.com/SanderMertens/flecs with native dynamic link libraries.

## How to use

### From source

1. Download and install [.NET 6](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/flecs-hub/flecs-cs`.
3. Build the native library by running `library.sh`. To execute `.sh` scripts on Windows, use Git Bash which can be installed with Git itself: https://git-scm.com/download/win. The `library.sh` script requires that CMake is installed and in your path.
4. To setup everything you need: Either (1), add the `src/cs/production/Flecs/Flecs.csproj` C# project to your solution as an existing project and reference it within your own solution, or (2) import the MSBuild `flecs.props` file which is located in the root of this directory to your `.csproj` file. See the [flecs.csproj](https://github.com/flecs-hub/flecs-cs/blob/main/src/cs/production/Flecs/Flecs.csproj) file for how to import the `flecs.props` directly.

## Developers: Documentation

For more information on how C# bindings work, see [`C2CS`](https://github.com/lithiumtoast/c2cs), the tool that generates the bindings for `flecs` and other C libraries.

To learn how to use `flecs`, check out the https://github.com/SanderMertens/flecs#documentation.

## License

`flecs-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`flecs` itself is licensed under MIT (`MIT`) - see https://github.com/SanderMertens/flecs/blob/master/LICENSE for more details.
