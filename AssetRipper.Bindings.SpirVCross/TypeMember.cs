using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct TypeMember
{
	public uint DeclaringTypeId { get; }
	public uint Index { get; }
	public Compiler Compiler { get; }
	public bool IsNull => Compiler.IsNull;

	public uint ArrayStride => IsNull ? default : Compiler.TypeStructMemberArrayStride(DeclaringType, Index);

	public Type DeclaringType => IsNull ? default : Compiler.GetTypeHandle(DeclaringTypeId);

	public IEnumerable<Decoration> Decorations
	{
		get
		{
			if (IsNull)
			{
				yield break;
			}
			foreach (Decoration decoration in Enum.GetValues<Decoration>())
			{
				if (HasDecoration(decoration))
				{
					yield return decoration;
				}
			}
		}
	}

	public uint MatrixStride => IsNull ? default : Compiler.TypeStructMemberMatrixStride(DeclaringType, Index);

	public uint MemberTypeId => IsNull ? default : DeclaringType.GetMemberType(Index);

	public Type MemberType => IsNull ? default : Compiler.GetTypeHandle(MemberTypeId);

	public string? Name
	{
		get => IsNull ? null : Compiler.GetMemberNameS(DeclaringTypeId, Index);
		set
		{
			if (!IsNull)
			{
				Compiler.SetMemberName(DeclaringTypeId, Index, value);
			}
		}
	}

	public uint Offset => IsNull ? default : Compiler.TypeStructMemberOffset(DeclaringType, Index);

	public nuint Size
	{
		get
		{
			if (IsNull)
			{
				return default;
			}
			else
			{
				return Compiler.GetDeclaredStructMemberSize(DeclaringType, Index);
			}
		}
	}

	internal TypeMember(uint declaringTypeId, uint index, Compiler compiler)
	{
		DeclaringTypeId = declaringTypeId;
		Index = index;
		Compiler = compiler;
	}

	public uint GetDecoration(Decoration decoration)
	{
		return IsNull ? default : Compiler.GetMemberDecoration(DeclaringTypeId, Index, decoration);
	}

	public uint? GetDecorationOrNull(Decoration decoration)
	{
		return HasDecoration(decoration) ? GetDecoration(decoration) : null;
	}

	public string? GetDecorationString(Decoration decoration)
	{
		return IsNull ? null : Compiler.GetMemberDecorationStringS(DeclaringTypeId, Index, decoration);
	}

	public bool HasDecoration(Decoration decoration)
	{
		return !IsNull && Compiler.HasMemberDecoration(DeclaringTypeId, Index, decoration);
	}

	public void SetDecoration(Decoration decoration, uint value)
	{
		if (!IsNull)
		{
			Compiler.SetMemberDecoration(DeclaringTypeId, Index, decoration, value);
		}
	}

	public void SetDecorationString(Decoration decoration, string value)
	{
		if (!IsNull)
		{
			Compiler.SetMemberDecorationString(DeclaringTypeId, Index, decoration, value);
		}
	}

	public void UnsetDecoration(Decoration decoration)
	{
		if (!IsNull)
		{
			Compiler.UnsetMemberDecoration(DeclaringTypeId, Index, decoration);
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
			return $"{nameof(TypeMember)} {DeclaringTypeId} : {Index}";
		}
	}
}
