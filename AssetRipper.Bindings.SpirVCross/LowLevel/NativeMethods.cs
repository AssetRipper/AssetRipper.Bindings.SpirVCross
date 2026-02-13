namespace AssetRipper.Bindings.SpirVCross.LowLevel;

public static unsafe partial class NativeMethods
{
	public unsafe static CompilerOptions CompilerCreateCompilerOptions(Compiler compiler)
	{
		OpaqueCompilerOptions* options = default;
		PInvoke.CompilerCreateCompilerOptions(compiler, &options).ThrowIfError(compiler.Context);
		return new CompilerOptions(options, compiler);
	}

	public unsafe static Resources CompilerCreateShaderResources(Compiler compiler)
	{
		OpaqueResources* resources = default;
		PInvoke.CompilerCreateShaderResources(compiler, &resources).ThrowIfError(compiler.Context);
		return new Resources(resources, compiler);
	}

	public unsafe static Resources CompilerCreateShaderResourcesForActiveVariables(Compiler compiler, Set active)
	{
		OpaqueResources* resources = default;
		PInvoke.CompilerCreateShaderResourcesForActiveVariables(compiler, &resources, active).ThrowIfError(compiler.Context);
		return new Resources(resources, compiler);
	}

	public unsafe static Set CompilerGetActiveInterfaceVariables(Compiler compiler)
	{
		OpaqueSet* set = default;
		PInvoke.CompilerGetActiveInterfaceVariables(compiler, &set).ThrowIfError(compiler.Context);
		return new Set(set);
	}

	public unsafe static Constant CompilerGetConstantHandle(Compiler compiler, uint id)
	{
		OpaqueConstant* pointer = PInvoke.CompilerGetConstantHandle(compiler, id);
		return new Constant(pointer, compiler, id);
	}

	public unsafe static Type CompilerGetTypeHandle(Compiler compiler, uint id)
	{
		OpaqueType* pointer = PInvoke.CompilerGetTypeHandle(compiler, id);
		return new Type(pointer, compiler, id);
	}

	public unsafe static Context ContextCreate()
	{
		OpaqueContext* context = default;
		PInvoke.ContextCreate(&context).ThrowIfError(default);
		return new Context(context);
	}

	public unsafe static Compiler ContextCreateCompiler(Context context, Backend backend, ParsedIR parsed_ir, CaptureMode mode)
	{
		OpaqueCompiler* compiler = default;
		PInvoke.ContextCreateCompiler(context, backend, parsed_ir, mode, &compiler).ThrowIfError(context);
		return new Compiler(compiler, context);
	}

	public unsafe static ParsedIR ContextParseSpirv(Context context, uint* spirv, nuint word_count)
	{
		OpaqueParsedIR* parsed_ir = default;
		PInvoke.ContextParseSpirv(context, spirv, word_count, &parsed_ir).ThrowIfError(context);
		return new ParsedIR(parsed_ir);
	}
}
