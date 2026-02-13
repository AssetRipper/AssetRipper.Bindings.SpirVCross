using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public unsafe readonly partial struct ParsedIR
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueParsedIR* Handle { get; }
	public bool IsNull => Handle is null;
	public ParsedIR(OpaqueParsedIR* pointer)
	{
		Handle = pointer;
	}
	public void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException("Handle is null.");
		}
	}
}
