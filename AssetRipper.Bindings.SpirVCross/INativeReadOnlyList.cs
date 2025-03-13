namespace AssetRipper.Bindings.SpirVCross;

public unsafe interface INativeReadOnlyList<T> where T : unmanaged
{
	int Count { get; }
	T this[int index] { get; }
}
