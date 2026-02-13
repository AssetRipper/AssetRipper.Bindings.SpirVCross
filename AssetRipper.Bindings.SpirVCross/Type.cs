using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public unsafe readonly partial struct Type
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueType* Handle { get; }
	public Compiler Compiler { get; }
	internal uint Id { get; }
	public Context Context => Compiler.Context;
	public bool IsNull => Handle is null;

	public readonly string? Name
	{
		get
		{
			return IsNull ? null : Compiler.GetNameS(Id);
		}
		set
		{
			if (!IsNull && !string.IsNullOrEmpty(value))
			{
				Compiler.SetName(Id, value);
			}
		}
	}
	public Type BaseType
	{
		get
		{
			return IsNull ? default : Compiler.GetTypeHandle(GetBaseTypeId());
		}
	}
	public Basetype BaseTypeEnum
	{
		get
		{
			return IsNull ? default : GetBasetype();
		}
	}
	public uint BitWidth
	{
		get
		{
			return IsNull ? 0 : GetBitWidth();
		}
	}
	public uint VectorSize
	{
		get
		{
			return IsNull ? 0 : GetVectorSize();
		}
	}
	public uint Columns
	{
		get
		{
			return IsNull ? 0 : GetColumns();
		}
	}
	public uint NumArrayDimensions
	{
		get
		{
			return IsNull ? 0 : GetNumArrayDimensions();
		}
	}
	public uint[] ArrayDimensions
	{
		get
		{
			uint numDimensions = NumArrayDimensions;
			if (numDimensions == 0)
			{
				return [];
			}
			uint[] dimensions = new uint[numDimensions];
			for (uint i = 0; i < numDimensions; i++)
			{
				dimensions[i] = GetArrayDimension(i);
			}
			return dimensions;
		}
	}
	public uint NumMemberTypes
	{
		get
		{
			return IsNull ? 0 : GetNumMemberTypes();
		}
	}
	public string?[] MemberNames
	{
		get
		{
			uint numMemberTypes = NumMemberTypes;
			if (numMemberTypes == 0)
			{
				return [];
			}
			string?[] names = new string?[numMemberTypes];
			for (uint i = 0; i < numMemberTypes; i++)
			{
				names[i] = GetMemberName(i);
			}
			return names;
		}
	}
	public StorageClass StorageClass
	{
		get
		{
			return IsNull ? default : GetStorageClass();
		}
	}
	public Type ImageSampledType
	{
		get
		{
			return IsNull ? default : Compiler.GetTypeHandle(GetImageSampledType());
		}
	}
	public Dim ImageDimension
	{
		get
		{
			return IsNull ? default : GetImageDimension();
		}
	}
	public bool ImageIsDepth
	{
		get
		{
			return IsNull ? default : GetImageIsDepth() != 0;
		}
	}
	public bool ImageArrayed
	{
		get
		{
			return IsNull ? default : GetImageArrayed() != 0;
		}
	}
	public bool ImageMultisampled
	{
		get
		{
			return IsNull ? default : GetImageMultisampled() != 0;
		}
	}
	public bool ImageIsStorage
	{
		get
		{
			return IsNull ? default : GetImageIsStorage() != 0;
		}
	}
	public ImageFormat ImageStorageFormat
	{
		get
		{
			return IsNull ? default : GetImageStorageFormat();
		}
	}
	public AccessQualifier ImageAccessQualifier
	{
		get
		{
			return IsNull ? default : GetImageAccessQualifier();
		}
	}
	public nuint DeclaredStructSize
	{
		get
		{
			return IsNull ? 0 : Compiler.GetDeclaredStructSize(this);
		}
	}

	public string? GetMemberName(uint i)
	{
		return Compiler.GetMemberNameS(Id, i);
	}

	public void SetMemberName(uint i, string name)
	{
		Compiler.SetMemberName(Id, i, name);
	}

	public Type GetMemberTypeHandle(uint index)
	{
		return Compiler.GetTypeHandle(GetMemberType(index));
	}

	internal Type(OpaqueType* pointer, Compiler compiler, uint id)
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
			return $"{nameof(Type)} {Id}";
		}
	}
}
