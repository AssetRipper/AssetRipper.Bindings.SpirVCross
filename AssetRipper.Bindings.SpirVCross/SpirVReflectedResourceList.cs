using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerTypeProxy(typeof(SpirVReflectedResourceListDebugView))]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly unsafe ref struct SpirVReflectedResourceList
{
	public SpirVContext Context => Compiler.Context;
	public SpirVCompiler Compiler { get; }
	private ReadOnlySpan<ReflectedResource> Span { get; }
	public int Count => Span.Length;
	public readonly SpirVReflectedResource this[int index]
	{
		get
		{
			fixed (ReflectedResource* resource = &Span[index])
			{
				return new SpirVReflectedResource(Compiler, resource);
			}
		}
	}
	public SpirVReflectedResourceList(SpirVCompiler compiler, ReadOnlySpan<ReflectedResource> span)
	{
		Compiler = compiler;
		Span = span;
	}

	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	internal readonly ref struct SpirVReflectedResourceListDebugView
	{
		public int Count => Items.Length;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public SpirVReflectedResourceDebugView[] Items { get; }

		public SpirVReflectedResourceListDebugView(SpirVReflectedResourceList list)
		{
			Items = new SpirVReflectedResourceDebugView[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				Items[i] = new SpirVReflectedResourceDebugView(list[i]);
			}
		}

		[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
		public class SpirVReflectedResourceDebugView
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private Context* context;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private Compiler* compiler;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private ReflectedResource* resource;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public SpirVReflectedResource Proxy => new SpirVReflectedResource(new SpirVCompiler(new SpirVContext(context), compiler), resource);

			public SpirVReflectedResourceDebugView(SpirVReflectedResource resource)
			{
				context = resource.Context.Pointer;
				compiler = resource.Compiler.Pointer;
				this.resource = resource.Pointer;
			}

			private string GetDebuggerDisplay()
			{
				return Proxy.IsNull ? "null" : Proxy.Name ?? "";
			}
		}
	}

	private string GetDebuggerDisplay()
	{
		return $"{nameof(SpirVReflectedResourceList)} Count={Count}";
	}
}
