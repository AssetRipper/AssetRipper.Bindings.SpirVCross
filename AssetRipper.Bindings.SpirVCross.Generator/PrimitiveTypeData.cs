namespace AssetRipper.Bindings.SpirVCross.Generator;

internal sealed record class PrimitiveTypeData : TypeData
{
	public Type Type { get; }
	public override bool IsVoid => Type == typeof(void);
	public bool IsBoolean => Type == typeof(bool);
	public bool IsByte => Type == typeof(byte);
	public override string FullName => IsVoid ? "void" : $"global::{Type.FullName}";
	public PrimitiveTypeData(Type type)
	{
		Type = type;
	}
}
