using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public unsafe readonly partial struct Resources
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public OpaqueResources* Handle { get; }
	public Compiler Compiler { get; }
	public Context Context => Compiler.Context;
	public bool IsNull => Handle is null;
	internal Resources(OpaqueResources* pointer, Compiler compiler)
	{
		Handle = pointer;
		Compiler = compiler;
	}
	public void ThrowIfNull()
	{
		if (IsNull)
		{
			throw new NullReferenceException("Handle is null.");
		}
	}

	public SpirVReflectedResourceList GetResourceListForType(ResourceType type)
	{
		ThrowIfNull();
		ReflectedResource* firstElement = null;
		nuint count = 0;
		GetResourceListForType(type, &firstElement, &count);
		return new(Compiler, SpanHelper.CreateReadOnlySpan(firstElement, (int)count));
	}

	public SpirVReflectedBuiltinResourceList GetBuiltinResourceListForType(BuiltinResourceType type)
	{
		ThrowIfNull();
		ReflectedBuiltinResource* firstElement = null;
		nuint count = 0;
		GetBuiltinResourceListForType(type, &firstElement, &count);
		return new(Compiler, SpanHelper.CreateReadOnlySpan(firstElement, (int)count));
	}
}
