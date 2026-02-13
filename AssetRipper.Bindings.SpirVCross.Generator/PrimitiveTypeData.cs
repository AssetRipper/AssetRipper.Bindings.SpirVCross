namespace AssetRipper.Bindings.SpirVCross.Generator;

internal sealed record class PrimitiveTypeData : TypeData
{
	public Type Type { get; }
	public override bool IsVoid => Type == typeof(void);
	public override string FullName => IsVoid ? "void" : $"global::{Type.FullName}";
	public PrimitiveTypeData(Type type)
	{
		Type = type;
	}
}
