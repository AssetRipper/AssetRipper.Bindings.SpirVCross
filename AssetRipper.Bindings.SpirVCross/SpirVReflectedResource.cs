using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVReflectedResource
{
	public Context Context => Compiler.Context;

	public Compiler Compiler { get; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ReflectedResource* Pointer { get; }

	public bool IsNull => Pointer == null;

	public Type BaseType => IsNull ? default : Compiler.GetTypeHandle(Pointer->BaseTypeId);

	public uint BaseTypeId => IsNull ? default : Pointer->BaseTypeId;

	public uint? Binding => IsNull ? null : AsDecoratedObject().GetDecorationOrNull(Decoration.Binding);

	public IEnumerable<Decoration> Decorations => AsDecoratedObject().Decorations;

	public uint Id => IsNull ? default : Pointer->Id;

	public uint? Location => AsDecoratedObject().GetDecorationOrNull(Decoration.Location);

	[DisallowNull]
	public string? Name
	{
		get => AsDecoratedObject().Name;
		set => AsDecoratedObject().Name = value;
	}

	public readonly Type Type => IsNull ? default : Compiler.GetTypeHandle(Pointer->TypeId);

	public uint TypeId => IsNull ? default : Pointer->TypeId;

	internal SpirVReflectedResource(Compiler compiler, ReflectedResource* pointer)
	{
		Compiler = compiler;
		Pointer = pointer;
	}

	public DecoratedObject AsDecoratedObject() => IsNull ? default : new DecoratedObject(Pointer->Id, Compiler);

	public void SetSemantic(string semantic)
	{
		Compiler.AddVertexAttributeRemap(Location ?? throw new InvalidOperationException("Resource is not decorated with a location"), semantic);
	}

	private void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException();
		}
	}
}
