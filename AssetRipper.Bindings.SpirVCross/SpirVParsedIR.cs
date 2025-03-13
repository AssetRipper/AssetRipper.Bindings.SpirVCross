using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVParsedIR : INativeStruct<ParsedIr>
{
	public SpirVContext Context { get; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ParsedIr* Pointer { get; }

	public bool IsNull => Pointer == null;

	internal SpirVParsedIR(SpirVContext context, ParsedIr* pointer)
	{
		Context = context;
		Pointer = pointer;
	}
}
