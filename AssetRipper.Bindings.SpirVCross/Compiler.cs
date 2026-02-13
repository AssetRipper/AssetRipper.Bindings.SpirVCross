using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AssetRipper.Bindings.SpirVCross;

public unsafe readonly partial struct Compiler
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueCompiler* Handle { get; }
	public Context Context { get; }
	public bool IsNull => Handle is null;

	public uint CurrentIdBound => IsNull ? default : GetCurrentIdBound();

	public nuint NumRequiredExtensions => IsNull ? default : GetNumRequiredExtensions();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ReadOnlySpan<EntryPoint> EntryPoints
	{
		get
		{
			EntryPoint* firstElement = null;
			nuint count = 0;
			GetEntryPoints(&firstElement, &count);
			return firstElement == null ? default : new ReadOnlySpan<EntryPoint>(firstElement, (int)count);
		}
	}

	public ExecutionModel ExecutionModel => IsNull ? default : GetExecutionModel();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ReadOnlySpan<ExecutionMode> ExecutionModes
	{
		get
		{
			ExecutionMode* firstElement = null;
			nuint count = 0;
			GetExecutionModes(&firstElement, &count);
			return firstElement == null ? default : new ReadOnlySpan<ExecutionMode>(firstElement, (int)count);
		}
	}

	public bool this[ExecutionMode mode]
	{
		get
		{
			Debug.Assert(sizeof(ExecutionMode) == sizeof(int));
			return MemoryMarshal.Cast<ExecutionMode, int>(ExecutionModes).IndexOf((int)mode) >= 0;
		}
		set
		{
			if (value)
			{
				SetExecutionMode(mode);
			}
			else
			{
				UnsetExecutionMode(mode);
			}
		}
	}

	internal Compiler(OpaqueCompiler* pointer, Context context)
	{
		Handle = pointer;
		Context = context;
	}

	public void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException("Handle is null.");
		}
	}

	public void AddVertexAttributeRemap(HlslVertexAttributeRemap remap)
	{
		HlslAddVertexAttributeRemap(&remap, 1);
	}

	public void AddVertexAttributeRemap(uint location, string semantic)
	{
		ThrowIfNull();
		nint semanticPointer = Marshal.StringToHGlobalAnsi(semantic);
		try
		{
			AddVertexAttributeRemap(new HlslVertexAttributeRemap
			{
				Location = location,
				Semantic = (byte*)semanticPointer,
			});
		}
		finally
		{
			Marshal.FreeHGlobal(semanticPointer);
		}
	}

	public string? Compile()
	{
		byte* output = null;
		Compile(&output);
		return NativeString.ToString(output);
	}

	public Type GetMemberType(uint id, uint index)
	{
		return GetTypeHandle(id).GetMemberTypeHandle(index);
	}

	internal nuint GetDeclaredStructSize(Type type)
	{
		nuint size = 0;
		GetDeclaredStructSize(type, &size);
		return size;
	}
}
