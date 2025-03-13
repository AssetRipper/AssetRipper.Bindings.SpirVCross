using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVCompilerOptions
{
	public SpirVContext Context { get; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal CompilerOptions* Pointer { get; }

	public bool IsNull => Pointer == null;

	internal SpirVCompilerOptions(SpirVContext context, CompilerOptions* pointer)
	{
		Context = context;
		Pointer = pointer;
	}

	private void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException();
		}
	}

	public void SetOption(CompilerOption option, bool value)
	{
		ThrowIfNull();
		SpirVCrossNative.CompilerOptionsSetBool(Pointer, option, value ? (byte)1 : (byte)0).ThrowIfError(Context);
	}

	public void SetOption(CompilerOption option, uint value)
	{
		ThrowIfNull();
		SpirVCrossNative.CompilerOptionsSetUint(Pointer, option, value).ThrowIfError(Context);
	}
}
