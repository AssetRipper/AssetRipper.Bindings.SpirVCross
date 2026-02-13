namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpecializationConstantList
{
	public Compiler Compiler { get; }
	private ReadOnlySpan<SpecializationConstant> Span { get; }

	internal SpecializationConstantList(Compiler compiler, SpecializationConstant* firstElement, nuint count)
	{
		Compiler = compiler;
		Span = new ReadOnlySpan<SpecializationConstant>(firstElement, (int)count);
	}
}
