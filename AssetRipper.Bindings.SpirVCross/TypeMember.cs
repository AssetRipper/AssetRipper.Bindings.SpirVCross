using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct TypeMember
{
	public uint DeclaringTypeId { get; }
	public uint Index { get; }
	public Compiler Compiler { get; }
	public bool IsNull => Compiler.IsNull;

	public Type DeclaringType => IsNull ? default : Compiler.GetTypeHandle(DeclaringTypeId);

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
