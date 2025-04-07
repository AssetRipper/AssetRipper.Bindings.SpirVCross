using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVReflectedResource : INativeStruct<ReflectedResource>
{
	public SpirVContext Context => Compiler.Context;

	public SpirVCompiler Compiler { get; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ReflectedResource* Pointer { get; }

	public bool IsNull => Pointer == null;

	public uint VariableId
	{
		get
		{
			ThrowIfNull();
			return Pointer->Id;
		}
	}

	public string? VariableName
	{
		get => IsNull ? null : Compiler.GetName(Pointer->Id);
		set
		{
			if (!IsNull && !string.IsNullOrEmpty(value))
			{
				Compiler.SetName(Pointer->Id, value);
			}
		}
	}

	public SpirVType BaseType
	{
		get
		{
			ThrowIfNull();
			return Compiler.GetType(Pointer->BaseTypeId);
		}
	}

	public readonly SpirVType Type
	{
		get
		{
			ThrowIfNull();
			return Compiler.GetType(Pointer->TypeId);
		}
	}

	public string? Name
	{
		get
		{
			ThrowIfNull();
			return NativeString.ToString(Pointer->Name);
		}
	}

	public uint Location
	{
		get => GetDecoration(Decoration.Location);
		set => SetDecoration(Decoration.Location, value);
	}

	public uint GetDecoration(Decoration decoration)
	{
		return Compiler.GetDecoration(VariableId, decoration);
	}

	public void SetDecoration(Decoration decoration, uint value)
	{
		Compiler.SetDecoration(VariableId, decoration, value);
	}

	public string? GetDecorationString(Decoration decoration)
	{
		return Compiler.GetDecorationString(VariableId, decoration);
	}

	public void SetDecorationString(Decoration decoration, string? value)
	{
		Compiler.SetDecorationString(VariableId, decoration, value);
	}

	public void SetSemantic(string semantic)
	{
		Compiler.AddVertexAttributeRemap(Location, semantic);
	}

	internal SpirVReflectedResource(SpirVCompiler compiler, ReflectedResource* pointer)
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
