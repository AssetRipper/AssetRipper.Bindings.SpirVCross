namespace AssetRipper.Bindings.SpirVCross;

public static partial class SpirVCrossNative
{
	[field: ThreadStatic]
	private static Cross ApiInstance => field ??= Cross.GetApi();
}
