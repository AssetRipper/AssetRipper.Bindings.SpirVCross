using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public unsafe readonly partial struct Constant
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueConstant* Handle { get; }
	public Compiler Compiler { get; }
	internal uint Id { get; }
	public Context Context => Compiler.Context;
	public bool IsNull => Handle is null;
	public Type Type
	{
		get
		{
			return IsNull ? default : Compiler.GetTypeHandle(GetTypeId());
		}
	}
	public Constant(OpaqueConstant* pointer, Compiler compiler, uint id)
	{
		Handle = pointer;
		Compiler = compiler;
		Id = id;
	}
	public void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException("Handle is null.");
		}
	}
	private string GetDebuggerDisplay()
	{
		if (IsNull)
		{
			return "null";
		}
		else
		{
			return $"{nameof(Constant)} {Id}";
		}
	}
}
