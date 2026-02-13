using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

[DebuggerTypeProxy(typeof(SpirVReflectedResourceListDebugView))]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly unsafe ref struct SpirVReflectedResourceList
{
	public Context Context => Compiler.Context;
	public Compiler Compiler { get; }
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
	public SpirVReflectedResourceList(Compiler compiler, ReadOnlySpan<ReflectedResource> span)
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
			private Compiler compiler;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private ReflectedResource* resource;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public SpirVReflectedResource Proxy => new SpirVReflectedResource(compiler, resource);

			public SpirVReflectedResourceDebugView(SpirVReflectedResource resource)
			{
				compiler = resource.Compiler;
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
