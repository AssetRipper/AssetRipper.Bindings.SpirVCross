using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVContext : INativeStruct<Context>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public Context* Pointer { get; }

	public bool IsNull => Pointer == null;

	internal SpirVContext(Context* pointer)
	{
		Pointer = pointer;
	}

	private void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException();
		}
	}

	/// <summary>
	/// Use this to create a new context. You must call <see cref="Destroy"/> when you are done with it; use a try/finally block to ensure this.
	/// </summary>
	/// <returns>The newly created context.</returns>
	public static SpirVContext Create()
	{
		Context* context = null;
		SpirVCrossNative.ContextCreate(&context);
		return new SpirVContext(context);
	}

	/// <summary>
	/// This cleans up the context. You must call this when you are done with the context.
	/// </summary>
	/// <remarks>
	/// This should be called from a finally block to ensure that the context is destroyed.
	/// </remarks>
	public void Destroy()
	{
		ThrowIfNull();
		SpirVCrossNative.ContextDestroy(Pointer);
	}

	public SpirVParsedIR ParseSpirV(ReadOnlySpan<uint> words)
	{
		ThrowIfNull();
		ParsedIr* ir = null;
		fixed (uint* wordPtr = &words[0])
		{
			SpirVCrossNative.ContextParseSpirv(Pointer, wordPtr, (nuint)words.Length, &ir).ThrowIfError(this);
		}
		return new SpirVParsedIR(this, ir);
	}

	public SpirVCompiler ParseSpirV(ReadOnlySpan<uint> words, Backend target)
	{
		SpirVParsedIR ir = ParseSpirV(words);
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
				byte* error = SpirVCrossNative.ContextGetLastErrorString(Pointer);
				return NativeString.ToString(error);
			}
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	SpirVContext INativeStruct<Context>.Context => this;

	public SpirVCompiler CreateCompiler(Backend backend, SpirVParsedIR ir, CaptureMode captureMode)
	{
		Compiler* compiler = null;
		SpirVCrossNative.ContextCreateCompiler(Pointer, backend, ir.Pointer, captureMode, &compiler).ThrowIfError(this);
		return new SpirVCompiler(this, compiler);
	}
}
