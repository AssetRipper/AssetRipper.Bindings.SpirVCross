using System.Runtime.InteropServices;

namespace AssetRipper.Bindings.SpirVCross;

internal static unsafe class NativeString
{
	public static string? ToString(byte* ptr)
	{
		return Marshal.PtrToStringAnsi((nint)ptr);
	}

	public static string ToStringNotNull(byte* ptr)
	{
		return Marshal.PtrToStringAnsi((nint)ptr) ?? "";
	}
}
