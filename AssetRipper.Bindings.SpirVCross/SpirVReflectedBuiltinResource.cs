using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVReflectedBuiltinResource
{
	public Context Context => Compiler.Context;

	public Compiler Compiler { get; }

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

	public Type ValueType
	{
		get
		{
			ThrowIfNull();
			return Compiler.GetTypeHandle(Pointer->ValueTypeId);
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

	internal SpirVReflectedBuiltinResource(Compiler compiler, ReflectedBuiltinResource* pointer)
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
