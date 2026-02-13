using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerTypeProxy(typeof(SpirVReflectedBuiltinResourceListDebugView))]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly unsafe ref struct SpirVReflectedBuiltinResourceList
{
	public Context Context => Compiler.Context;
	public Compiler Compiler { get; }
	private ReadOnlySpan<ReflectedBuiltinResource> Span { get; }
	public int Count => Span.Length;
	public SpirVReflectedBuiltinResource this[int index]
	{
		get
		{
			fixed (ReflectedBuiltinResource* resource = &Span[index])
			{
				return new SpirVReflectedBuiltinResource(Compiler, resource);
			}
		}
	}
	public SpirVReflectedBuiltinResourceList(Compiler compiler, ReadOnlySpan<ReflectedBuiltinResource> span)
	{
		Compiler = compiler;
		Span = span;
	}

	internal readonly ref struct SpirVReflectedBuiltinResourceListDebugView
	{
		public int Count => Items.Length;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public SpirVReflectedBuiltinDebugView[] Items { get; }

		public SpirVReflectedBuiltinResourceListDebugView(SpirVReflectedBuiltinResourceList list)
		{
			Items = new SpirVReflectedBuiltinDebugView[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				Items[i] = new SpirVReflectedBuiltinDebugView(list[i]);
			}
		}

		[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
		public class SpirVReflectedBuiltinDebugView
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly Compiler compiler;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly ReflectedBuiltinResource* resource;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public SpirVReflectedBuiltinResource Proxy => new SpirVReflectedBuiltinResource(compiler, resource);

			public SpirVReflectedBuiltinDebugView(SpirVReflectedBuiltinResource resource)
			{
				compiler = resource.Compiler;
				this.resource = resource.Pointer;
			}

			private string GetDebuggerDisplay()
			{
				return Proxy.IsNull ? "null" : Proxy.Resource.IsNull ? nameof(SpirVReflectedBuiltinResource) : Proxy.Resource.Name ?? "";
			}
		}
	}

	private string GetDebuggerDisplay()
	{
		return $"{nameof(SpirVReflectedBuiltinResourceList)} Count={Count}";
	}
}
