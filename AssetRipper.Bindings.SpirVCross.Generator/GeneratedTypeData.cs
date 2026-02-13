namespace AssetRipper.Bindings.SpirVCross.Generator;

internal sealed record class GeneratedTypeData : TypeData
{
	public string Namespace { get; }
	public string Name { get; }
	public override string FullName => $"global::{Namespace}.{Name}";
	public GeneratedTypeData(string @namespace, string name)
	{
		Namespace = @namespace;
		Name = name;
	}
}
