using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerTypeProxy(typeof(SpirVReflectedBuiltinResourceListDebugView))]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly unsafe ref struct SpirVReflectedBuiltinResourceList
{
	public SpirVContext Context => Compiler.Context;
	public SpirVCompiler Compiler { get; }
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
	public SpirVReflectedBuiltinResourceList(SpirVCompiler compiler, ReadOnlySpan<ReflectedBuiltinResource> span)
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
			private Context* context;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private Compiler* compiler;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private ReflectedBuiltinResource* resource;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public SpirVReflectedBuiltinResource Proxy => new SpirVReflectedBuiltinResource(new SpirVCompiler(new SpirVContext(context), compiler), resource);

			public SpirVReflectedBuiltinDebugView(SpirVReflectedBuiltinResource resource)
			{
				context = resource.Context.Pointer;
				compiler = resource.Compiler.Pointer;
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
