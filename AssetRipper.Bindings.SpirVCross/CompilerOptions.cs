using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public unsafe readonly partial struct CompilerOptions
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueCompilerOptions* Handle { get; }
	public Compiler Compiler { get; }
	public Context Context => Compiler.Context;
	public bool IsNull => Handle is null;
	internal CompilerOptions(OpaqueCompilerOptions* pointer, Compiler compiler)
	{
		Handle = pointer;
		Compiler = compiler;
	}
	public void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException("Handle is null.");
		}
	}

	public void SetOption(CompilerOption option, bool value)
	{
		SetBool(option, value ? (byte)1 : (byte)0);
	}

	public void SetOption(CompilerOption option, uint value)
	{
		SetUInt(option, value);
	}
}
