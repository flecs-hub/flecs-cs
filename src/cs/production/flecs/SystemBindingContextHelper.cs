using System;
using System.Threading;

namespace flecs;

internal static class SystemBindingContextHelper
{
    private static SystemBindingContextData[] _bindingContextInstances = new SystemBindingContextData[512];
    private static int _systemsCount;

   public static IntPtr CreateSystemBindingContext(SystemCallback callback)
    {
        var data = new SystemBindingContextData(callback);
        var count = Interlocked.Increment(ref _systemsCount);
        if (count > _bindingContextInstances.Length)
        {
            Array.Resize(ref _bindingContextInstances, count * 2);
        }
        
        _bindingContextInstances[count - 1] = data;
        var result = (IntPtr)count;
        return result;
    }

    public static void GetSystemBindingContext(IntPtr pointer, out SystemBindingContextData data)
    {
        var index = (int)pointer;
        data = _bindingContextInstances[index - 1];
    }
    
    public readonly struct SystemBindingContextData
    {
        public readonly SystemCallback Callback;

        public SystemBindingContextData(SystemCallback callback)
        {
            Callback = callback;
        }
    }
}
