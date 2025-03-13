using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpirVResources : INativeStruct<Resources>
{
	public SpirVContext Context => Compiler.Context;

	public SpirVCompiler Compiler { get; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public Resources* Pointer { get; }

	public bool IsNull => Pointer == null;

	internal SpirVResources(SpirVCompiler compiler, Resources* pointer)
	{
		Compiler = compiler;
		Pointer = pointer;
	}

	public SpirVReflectedResourceList GetResourceListForType(ResourceType type)
	{
		ThrowIfNull();
		ReflectedResource* firstElement = null;
		nuint count = 0;
		SpirVCrossNative.ResourcesGetResourceListForType(Pointer, type, &firstElement, &count).ThrowIfError(Context);
		return new(Compiler, SpanHelper.CreateReadOnlySpan(firstElement, (int)count));
	}

	public SpirVReflectedBuiltinResourceList GetBuiltinResourceListForType(BuiltinResourceType type)
	{
		ThrowIfNull();
		ReflectedBuiltinResource* firstElement = null;
		nuint count = 0;
		SpirVCrossNative.ResourcesGetBuiltinResourceListForType(Pointer, type, &firstElement, &count).ThrowIfError(Context);
		return new(Compiler, SpanHelper.CreateReadOnlySpan(firstElement, (int)count));
	}

	private void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException();
		}
	}
}
