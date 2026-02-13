using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public unsafe readonly partial struct Context : IDisposable
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueContext* Handle { get; }
	public bool IsNull => Handle is null;
	public string? LastErrorString => IsNull ? null : GetLastErrorStringS();

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

	public void Dispose()
	{
		if (!IsNull)
		{
			Destroy();
		}
	}
}
