namespace AssetRipper.Bindings.SpirVCross.Generator;

internal sealed record class PointerTypeData : TypeData
{
	public TypeData ElementType { get; }
	public override string FullName => $"{ElementType.FullName}*";
	public override TypeData RootType => ElementType.RootType;
	public PointerTypeData(TypeData elementType)
	{
		ElementType = elementType;
	}
}
