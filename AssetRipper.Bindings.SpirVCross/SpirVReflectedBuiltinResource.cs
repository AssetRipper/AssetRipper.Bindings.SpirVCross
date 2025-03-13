using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVReflectedBuiltinResource : INativeStruct<ReflectedBuiltinResource>
{
	public SpirVContext Context => Compiler.Context;

	public SpirVCompiler Compiler { get; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ReflectedBuiltinResource* Pointer { get; }

	public bool IsNull => Pointer == null;

	public BuiltIn BuiltIn
	{
		get
		{
			ThrowIfNull();
			return Pointer->Builtin;
		}
	}

	public SpirVType ValueType
	{
		get
		{
			ThrowIfNull();
			return Compiler.GetType(Pointer->ValueTypeId);
		}
	}

	public SpirVReflectedResource Resource
	{
		get
		{
			ThrowIfNull();
			return new SpirVReflectedResource(Compiler, &Pointer->Resource);
		}
	}

	internal SpirVReflectedBuiltinResource(SpirVCompiler compiler, ReflectedBuiltinResource* pointer)
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
