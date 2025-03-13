using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVConstant : INativeStruct<Constant>
{
	public SpirVContext Context => Compiler.Context;
	public SpirVCompiler Compiler { get; }
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public Constant* Pointer { get; }
	public bool IsNull => Pointer == null;
	public SpirVType Type
	{
		get
		{
			ThrowIfNull();
			return Compiler.GetType(SpirVCrossNative.ConstantGetType(Pointer));
		}
	}

	internal SpirVConstant(SpirVCompiler compiler, Constant* pointer)
	{
		Compiler = compiler;
		Pointer = pointer;
	}

	private void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException();
		}
	}
}
