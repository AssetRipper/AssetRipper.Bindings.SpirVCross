using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe struct SpirVEntryPoint
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal EntryPoint UnderlyingValue { get; }

	public ExecutionModel ExecutionModel => UnderlyingValue.ExecutionModel;

	public string? Name => NativeString.ToString(UnderlyingValue.Name);

	internal SpirVEntryPoint(EntryPoint entryPoint)
	{
		UnderlyingValue = entryPoint;
	}
}
