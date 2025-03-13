using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVCompiler : INativeStruct<Compiler>
{
	public SpirVContext Context { get; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public Compiler* Pointer { get; }

	public bool IsNull => Pointer == null;

	public uint CurrentIdBound
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.CompilerGetCurrentIdBound(Pointer);
		}
	}

	public nuint NumRequiredExtensions
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.CompilerGetNumRequiredExtensions(Pointer);
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ReadOnlySpan<SpirVEntryPoint> EntryPoints
	{
		get
		{
			Debug.Assert(sizeof(SpirVEntryPoint) == sizeof(EntryPoint));
			ThrowIfNull();
			EntryPoint* firstElement = null;
			nuint count = 0;
			SpirVCrossNative.CompilerGetEntryPoints(Pointer, &firstElement, &count).ThrowIfError(Context);
			return firstElement == null ? default : new ReadOnlySpan<SpirVEntryPoint>((SpirVEntryPoint*)firstElement, (int)count);
		}
	}

	public ExecutionModel ExecutionModel
	{
		get
		{
			ThrowIfNull();
			return SpirVCrossNative.CompilerGetExecutionModel(Pointer);
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ReadOnlySpan<ExecutionMode> ExecutionModes
	{
		get
		{
			ThrowIfNull();
			ExecutionMode* firstElement = null;
			nuint count = 0;
			SpirVCrossNative.CompilerGetExecutionModes(Pointer, &firstElement, &count).ThrowIfError(Context);
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
			ThrowIfNull();
			if (value)
			{
				SpirVCrossNative.CompilerSetExecutionMode(Pointer, mode);
			}
			else
			{
				SpirVCrossNative.CompilerUnsetExecutionMode(Pointer, mode);
			}
		}
	}

	internal SpirVCompiler(SpirVContext context, Compiler* pointer)
	{
		Context = context;
		Pointer = pointer;
	}

	private void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException();
		}
	}

	public void BuildCombinedImageSamplers()
	{
		ThrowIfNull();
		SpirVCrossNative.CompilerBuildCombinedImageSamplers(Pointer).ThrowIfError(Context);
	}

	public void UpdateActiveBuiltins()
	{
		ThrowIfNull();
		SpirVCrossNative.CompilerUpdateActiveBuiltins(Pointer);
	}

	public string? Compile()
	{
		ThrowIfNull();
		byte* output = null;
		SpirVCrossNative.CompilerCompile(Pointer, &output).ThrowIfError(Context);
		return NativeString.ToString(output);
	}

	public SpirVCompilerOptions CreateOptions()
	{
		ThrowIfNull();
		CompilerOptions* options = null;
		SpirVCrossNative.CompilerCreateCompilerOptions(Pointer, &options).ThrowIfError(Context);
		return new SpirVCompilerOptions(Context, options);
	}

	public void InstallOptions(SpirVCompilerOptions options)
	{
		ThrowIfNull();
		SpirVCrossNative.CompilerInstallCompilerOptions(Pointer, options.Pointer).ThrowIfError(Context);
	}

	public SpirVResources CreateShaderResources()
	{
		ThrowIfNull();
		Resources* resources = null;
		SpirVCrossNative.CompilerCreateShaderResources(Pointer, &resources).ThrowIfError(Context);
		return new SpirVResources(this, resources);
	}

	public string? GetName(uint id)
	{
		ThrowIfNull();
		byte* name = SpirVCrossNative.CompilerGetName(Pointer, id);
		return NativeString.ToString(name);
	}

	public void SetName(uint id, string name)
	{
		ThrowIfNull();
		nint namePointer = Marshal.StringToHGlobalAnsi(name);
		try
		{
			SpirVCrossNative.CompilerSetName(Pointer, id, (byte*)namePointer);
		}
		finally
		{
			Marshal.FreeHGlobal(namePointer);
		}
	}

	public string? GetMemberName(uint id, uint member)
	{
		ThrowIfNull();
		byte* name = SpirVCrossNative.CompilerGetMemberName(Pointer, id, member);
		return NativeString.ToString(name);
	}

	public void SetMemberName(uint id, uint member, string name)
	{
		ThrowIfNull();
		nint namePointer = Marshal.StringToHGlobalAnsi(name);
		try
		{
			SpirVCrossNative.CompilerSetMemberName(Pointer, id, member, (byte*)namePointer);
		}
		finally
		{
			Marshal.FreeHGlobal(namePointer);
		}
	}

	public SpirVType GetMemberType(uint id, uint index)
	{
		return GetType(id).GetMemberType(index);
	}

	internal SpirVConstant GetConstant(uint id)
	{
		ThrowIfNull();
		Constant* constant = SpirVCrossNative.CompilerGetConstantHandle(Pointer, id);
		return new SpirVConstant(this, constant);
	}

	internal SpirVType GetType(uint id)
	{
		ThrowIfNull();
		return new SpirVType(this, SpirVCrossNative.CompilerGetTypeHandle(Pointer, id), id);
	}

	internal nuint GetDeclaredStructSize(CrossType* type)
	{
		ThrowIfNull();
		nuint size = 0;
		SpirVCrossNative.CompilerGetDeclaredStructSize(Pointer, type, &size).ThrowIfError(Context);
		return size;
	}
}
