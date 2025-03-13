namespace AssetRipper.Bindings.SpirVCross;

public readonly unsafe ref struct SpecializationConstantList
{
	public SpirVCompiler Compiler { get; }
	private ReadOnlySpan<SpecializationConstant> Span { get; }

	internal SpecializationConstantList(SpirVCompiler compiler, SpecializationConstant* firstElement, nuint count)
	{
		Compiler = compiler;
		Span = new ReadOnlySpan<SpecializationConstant>(firstElement, (int)count);
	}
}
