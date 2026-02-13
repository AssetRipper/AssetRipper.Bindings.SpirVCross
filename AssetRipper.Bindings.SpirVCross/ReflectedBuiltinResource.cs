namespace AssetRipper.Bindings.SpirVCross;

public unsafe partial struct ReflectedBuiltinResource
{
	public Type GetValueType(Compiler compiler)
	{
		return compiler.GetTypeHandle(ValueTypeId);
	}
}
