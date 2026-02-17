namespace AssetRipper.Bindings.SpirVCross;

public readonly struct DecoratedObject
{
	public uint Id { get; }
	public Compiler Compiler { get; }
	public Context Context => Compiler.Context;
	public bool IsNull => Compiler.IsNull;

	public IEnumerable<Decoration> Decorations
	{
		get
		{
			if (IsNull)
			{
				yield break;
			}
			foreach (Decoration decoration in Enum.GetValues<Decoration>())
			{
				if (HasDecoration(decoration))
				{
					yield return decoration;
				}
			}
		}
	}

	public string? Name
	{
		get => IsNull ? null : Compiler.GetNameS(Id);
		set
		{
			if (!IsNull)
			{
				Compiler.SetName(Id, value);
			}
		}
	}

	public DecoratedObject(uint id, Compiler compiler)
	{
		Id = id;
		Compiler = compiler;
	}

	public uint GetDecoration(Decoration decoration)
	{
		return Compiler.GetDecoration(Id, decoration);
	}

	public uint? GetDecorationOrNull(Decoration decoration)
	{
		return HasDecoration(decoration) ? GetDecoration(decoration) : null;
	}

	public bool HasDecoration(Decoration decoration)
	{
		return Compiler.HasDecoration(Id, decoration);
	}

	public void SetDecoration(Decoration decoration, uint value)
	{
		Compiler.SetDecoration(Id, decoration, value);
	}

	public void UnsetDecoration(Decoration decoration)
	{
		Compiler.UnsetDecoration(Id, decoration);
	}

	public string? GetDecorationString(Decoration decoration)
	{
		return Compiler.GetDecorationStringS(Id, decoration);
	}

	public void SetDecorationString(Decoration decoration, string? value)
	{
		Compiler.SetDecorationString(Id, decoration, value);
	}
}
