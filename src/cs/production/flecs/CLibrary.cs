using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace flecs_hub;

public static class CLibrary
{
    public static IntPtr Load(string name)
    {
        if (IsLinux) return libdl.dlopen(name, 0x101); // RTLD_GLOBAL | RTLD_LAZY
        if (IsWindows) return Kernel32.LoadLibrary(name);
        if (IsDarwin) return libSystem.dlopen(name, 0x101); // RTLD_GLOBAL | RTLD_LAZY
        return IntPtr.Zero;
    }

    public static void Free(IntPtr handle)
    {
        if (IsLinux) libdl.dlclose(handle);
        if (IsWindows) Kernel32.FreeLibrary(handle);
        if (IsDarwin) libSystem.dlclose(handle);
    }

    public static IntPtr GetExport(IntPtr handle, string symbolName)
    {
        if (IsLinux) return libdl.dlsym(handle, symbolName);
        if (IsWindows) return Kernel32.GetProcAddress(handle, symbolName);
        if (IsDarwin) return libSystem.dlsym(handle, symbolName);
        return IntPtr.Zero;
    }

    private static bool IsWindows
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    private static bool IsDarwin
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }

    private static bool IsLinux
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    [SuppressUnmanagedCodeSecurity]
    public static class libdl
    {
        private const string LibraryName = "libdl";

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall)]
        public static extern int dlclose(IntPtr handle);
    }
    
    [SuppressUnmanagedCodeSecurity]
    internal static class libSystem
    {
        private const string LibraryName = "libSystem";

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall)]
        public static extern int dlclose(IntPtr handle);
    }
    
    [SuppressUnmanagedCodeSecurity]
    private static class Kernel32
    {
        [DllImport("kernel32", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int FreeLibrary(IntPtr module);
    }
}
