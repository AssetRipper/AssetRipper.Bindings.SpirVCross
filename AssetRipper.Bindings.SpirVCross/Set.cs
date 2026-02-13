using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public unsafe readonly partial struct Set
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueSet* Handle { get; }
	public bool IsNull => Handle is null;
	public Set(OpaqueSet* pointer)
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
