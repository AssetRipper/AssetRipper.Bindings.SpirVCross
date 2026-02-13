namespace AssetRipper.Bindings.SpirVCross.LowLevel;

public static partial class PInvoke
{
	[field: ThreadStatic]
	private static Cross ApiInstance => field ??= Cross.GetApi();
}
