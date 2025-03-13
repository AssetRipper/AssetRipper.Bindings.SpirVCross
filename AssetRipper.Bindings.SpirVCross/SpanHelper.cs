namespace AssetRipper.Bindings.SpirVCross;

internal static unsafe class SpanHelper
{
	public static ReadOnlySpan<T> CreateReadOnlySpan<T>(T* pointer, int length) where T : unmanaged
	{
		return pointer == null || length == 0 ? default : new ReadOnlySpan<T>(pointer, length);
	}
}
