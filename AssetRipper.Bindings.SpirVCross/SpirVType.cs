using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly unsafe ref struct SpirVType : INativeStruct<CrossType>
{
	public SpirVContext Context => Compiler.Context;
	public SpirVCompiler Compiler { get; }
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public CrossType* Pointer { get; }
	internal uint Id { get; }
	public bool IsNull => Pointer == null;
	public readonly string? Name
	{
		get
		{
			return IsNull ? null : Compiler.GetName(Id);
		}
		set
		{
			if (!IsNull && !string.IsNullOrEmpty(value))
			{
				Compiler.SetName(Id, value);
			}
		}
	}
	public SpirVType BaseType
	{
		get
		{
			ThrowIfNull();
			return Compiler.GetType(SpirVCrossNative.TypeGetBaseTypeId(Pointer));
		}
	}
	public Basetype BaseTypeEnum
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetBasetype(Pointer);
		}
	}
	public uint BitWidth
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetBitWidth(Pointer);
		}
	}
	public uint VectorSize
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetVectorSize(Pointer);
		}
	}
	public uint Columns
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetColumns(Pointer);
		}
	}
	public uint NumArrayDimensions
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetNumArrayDimensions(Pointer);
		}
	}
	public uint NumMemberTypes
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetNumMemberTypes(Pointer);
		}
	}
	public string?[] MemberNames
	{
		get
		{
			ThrowIfNull();
			string?[] names = new string?[NumMemberTypes];
			for (uint i = 0; i < NumMemberTypes; i++)
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
			ThrowIfNull();
			return SpirVCrossNative.TypeGetStorageClass(Pointer);
		}
	}
	public SpirVType ImageSampledType
	{
		get
		{
			ThrowIfNull();
			return Compiler.GetType(SpirVCrossNative.TypeGetImageSampledType(Pointer));
		}
	}
	public Dim ImageDimension
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetImageDimension(Pointer);
		}
	}
	public bool ImageIsDepth
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetImageIsDepth(Pointer) != 0;
		}
	}
	public bool ImageArrayed
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetImageArrayed(Pointer) != 0;
		}
	}
	public bool ImageMultisampled
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetImageMultisampled(Pointer) != 0;
		}
	}
	public bool ImageIsStorage
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetImageIsStorage(Pointer) != 0;
		}
	}
	public ImageFormat ImageStorageFormat
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetImageStorageFormat(Pointer);
		}
	}
	public AccessQualifier ImageAccessQualifier
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.TypeGetImageAccessQualifier(Pointer);
		}
	}
	public nuint DeclaredStructSize
	{
		get
		{
			ThrowIfNull();
			return Compiler.GetDeclaredStructSize(Pointer);
		}
	}

	public string? GetMemberName(uint i)
	{
		return Compiler.GetMemberName(Id, i);
	}

	public void SetMemberName(uint i, string name)
	{
		Compiler.SetMemberName(Id, i, name);
	}

	public SpirVType GetMemberType(uint index)
	{
		ThrowIfNull();
		return Compiler.GetType(SpirVCrossNative.TypeGetMemberType(Pointer, index));
	}

	internal SpirVType(SpirVCompiler compiler, CrossType* pointer, uint id)
	{
		Compiler = compiler;
		Pointer = pointer;
		Id = id;
	}

	private void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException();
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
			return $"{nameof(SpirVType)} {Id}";
		}
	}
}
