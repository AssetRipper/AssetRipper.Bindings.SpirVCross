namespace AssetRipper.Bindings.SpirVCross;

public unsafe partial struct EntryPoint
{
	public string? NameString => NativeString.ToString(Name);
}
