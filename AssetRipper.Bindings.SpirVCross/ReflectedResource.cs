namespace AssetRipper.Bindings.SpirVCross;

public unsafe partial struct ReflectedResource
{
	public string? NameString => NativeString.ToString(Name);
}
