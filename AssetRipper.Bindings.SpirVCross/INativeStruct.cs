namespace AssetRipper.Bindings.SpirVCross;

public unsafe interface INativeStruct<TNative>
	where TNative : unmanaged
{
	SpirVContext Context { get; }
	TNative* Pointer { get; }
}
public static unsafe class NativeStructExtensions
{
	public static bool IsNull<TSelf, TNative>(this TSelf self)
		where TSelf : INativeStruct<TNative>
		where TNative : unmanaged
	{
		return self.Pointer == null;
	}

	public static void ThrowIfNull<TSelf, TNative>(this TSelf self)
		where TSelf : INativeStruct<TNative>
		where TNative : unmanaged
	{
		if (self.IsNull<TSelf, TNative>())
		{
			throw new NullReferenceException();
		}
	}
}
