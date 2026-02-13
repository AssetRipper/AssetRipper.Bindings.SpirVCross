namespace AssetRipper.Bindings.SpirVCross.Generator;

internal sealed record class SilkNetTypeData : TypeData
{
	public Type Type { get; }
	public override string FullName => $"global::{Type.FullName.Replace('+', '.')}";
	public SilkNetTypeData(Type type)
	{
		Type = type;
	}
}
