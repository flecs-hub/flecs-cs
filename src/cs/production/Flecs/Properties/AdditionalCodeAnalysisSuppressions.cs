using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Microsoft.Reliability",
    "CA2020:Prevent behavioral change",
    MessageId = "Prevent behavioral change caused by built-in operators of IntPtr/UIntPtr",
    Justification = "The new changes from .NET 6 to .NET 7 for IntPtr/UIntPtr are acceptable.")]

[assembly: SuppressMessage(
    "Microsoft.Performance",
    "CA1815:Override equals and operator equals on value types",
    MessageId = "Override equals and operator equals on value types",
    Justification = "Any time we are using structs for Interop or are creating structs that wrap Interop structs we most definitely want them to be blittable to which rule this does not apply.")]

[assembly: SuppressMessage(
    "Microsoft.Design",
    "CA1040:Avoid empty interfaces",
    MessageId = "Avoid empty interfaces",
    Justification = "We are using interfaces with generics for compile time markers; this is acceptable according to MSDN.")]
