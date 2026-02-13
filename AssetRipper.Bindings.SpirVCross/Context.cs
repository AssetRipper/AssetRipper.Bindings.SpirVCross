using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public unsafe readonly partial struct Context
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueContext* Handle { get; }
	public bool IsNull => Handle is null;
	public Context(OpaqueContext* pointer)
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

	public ParsedIR ParseSpirv(ReadOnlySpan<uint> words)
	{
		fixed (uint* wordPtr = &words[0])
		{
			return ParseSpirv(wordPtr, (nuint)words.Length);
		}
	}

	public Compiler ParseSpirv(ReadOnlySpan<uint> words, Backend target)
	{
		ParsedIR ir = ParseSpirv(words);
		return CreateCompiler(target, ir, CaptureMode.TakeOwnership);
	}

	public string? LastErrorString
	{
		get
		{
			if (IsNull)
			{
				return null;
			}
			else
			{
				return GetLastErrorStringS();
			}
		}
	}
}
