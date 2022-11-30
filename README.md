# flecs-cs

Automatically updated C# bindings for https://github.com/SanderMertens/flecs with native dynamic link libraries. Includes the lower-level unsafe C# "binding" which is 100% automatically generated. The higher level safe "wrapper" is still being worked on.

## How to use

### From source

1. Download and install [.NET 7](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/flecs-hub/flecs-cs`.
3. Build the native library by running `library.sh`. To execute `.sh` scripts on Windows, use Git Bash which can be installed with Git itself: https://git-scm.com/download/win. The `library.sh` script requires that CMake is installed and in your path.
4. To setup everything you need: Either (1), add the `src/cs/production/Flecs/Flecs.csproj` C# project to your solution as an existing project and reference it within your own solution, or (2) import the MSBuild `flecs.props` file which is located in the root of this directory to your `.csproj` file. See the [flecs.csproj](https://github.com/flecs-hub/flecs-cs/blob/main/src/cs/production/Flecs/Flecs.csproj) file for how to import the `flecs.props` directly.

### Generate binding / wrapper C# code from `flecs.h` C header file (automatically done by GitHub actions)

1. Download and install [.NET 7](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/flecs-hub/flecs-cs`.
3. Install the lastest version of C2CS tool: `dotnet tool install -g bottlenoselabs.C2CS.Tool`
3. Use C2CS to extract the abstract syntax trees for either Windows, macOS, or Linux: `cd ./bindgen && read_c_code.sh`. It's recommended that you do this once on a Windows machine with C/C++ SDK, once on a macOS machine with XCode / CommandLineTools, and once on a Linux machine with dev tools. For more information on why and how this works see https://github.com/lithiumtoast/c2cs.
4. Move the resulting abstract syntax trees `.json` files from the previous step in the `./bindgen/ast/` directory across machines onto a single machine (any OS). 
5. Use C2CS to generate the C# code from the abstract syntax trees: `cd ./bindgen && write_csharp_code.sh`. If all is well you should now have an updated `./src/cs/production/Flecs/flecs.cs` file.

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
