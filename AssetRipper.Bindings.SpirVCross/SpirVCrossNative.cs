namespace AssetRipper.Bindings.SpirVCross;

public static partial class SpirVCrossNative
{
	[ThreadStatic]
	private static Cross? _apiInstance;

	private static Cross ApiInstance => _apiInstance ??= Cross.GetApi();
}
