namespace AssetRipper.Bindings.SpirVCross.Generator;

internal sealed record class ArrayTypeData : TypeData
{
	public TypeData ElementType { get; }
	public override string FullName => $"{ElementType.FullName}[]";
	public override TypeData RootType => ElementType.RootType;
	public ArrayTypeData(TypeData elementType)
	{
		ElementType = elementType;
	}
}
