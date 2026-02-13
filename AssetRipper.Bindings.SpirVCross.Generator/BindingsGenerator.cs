using Microsoft.CodeAnalysis;
using SGF;
using Silk.NET.SPIRV.Cross;
using System.Reflection;

namespace AssetRipper.Bindings.SpirVCross.Generator;

[IncrementalGenerator]
public partial class BindingsGenerator() : IncrementalGenerator(nameof(BindingsGenerator))
{
	private const string Namespace = "AssetRipper.Bindings.SpirVCross";
	private const string LowLevelNamespace = "AssetRipper.Bindings.SpirVCross.LowLevel";
	private const string ClassName = "SpirVCrossNative";
	private const string PInvokeClassName = "PInvoke";
	private const string PInvokeGlobalName = $"global::{LowLevelNamespace}.{PInvokeClassName}";
	private const string NativeMethodsClassName = "NativeMethods";
	private const string NativeMethodsGlobalName = $"global::{LowLevelNamespace}.{NativeMethodsClassName}";

	private static Dictionary<string, MethodData> ManuallyImplementedNativeMethods { get; } = GetManuallyImplementedNativeMethods().ToDictionary(m => m.Name);

	public override void OnInitialize(SgfInitializationContext context)
	{
		context.RegisterPostInitializationOutput(Generate);
	}

	private static void Generate(IncrementalGeneratorPostInitializationContext context)
	{
		List<MethodData> methods = GetAllMethods(typeof(Cross)).Select(MethodData.FromRuntimeMethod).ToList();
		methods.Sort();
		GenerateConstants(context);
		GenerateTypes(
			context,
			methods,
			out HashSet<GeneratedTypeData> generatedStructs,
			out List<(SilkNetTypeData, GeneratedTypeData)> needsMembers,
			out List<(SilkNetTypeData, GeneratedTypeData)> opaqueTypes,
			out Dictionary<TypeData, TypeData> replacements);
		GeneratePInvokeMethods(context, methods, replacements);
		GenerateWrapperTypes(context, opaqueTypes, generatedStructs, replacements, out HashSet<GeneratedTypeData> wrapperStructs);
		GenerateStructFields(context, needsMembers, replacements);
		GenerateNativeMethods(context, methods, replacements);
		GenerateStructMethods(context, methods, generatedStructs, wrapperStructs);
		GenerateMainMethods(context, methods);
	}

	private static MethodData[] GetManuallyImplementedNativeMethods()
	{
		// Wrapper structs
		GeneratedTypeData compiler = new(Namespace, "Compiler");
		GeneratedTypeData compilerOptions = new(Namespace, "CompilerOptions");
		GeneratedTypeData constant = new(Namespace, "Constant");
		GeneratedTypeData context = new(Namespace, "Context");
		GeneratedTypeData parsedIr = new(Namespace, "ParsedIR");
		GeneratedTypeData resources = new(Namespace, "Resources");
		GeneratedTypeData set = new(Namespace, "Set");
		GeneratedTypeData type = new(Namespace, "Type");

		// Other
		GeneratedTypeData backend = new(Namespace, "Backend");
		GeneratedTypeData captureMode = new(Namespace, "CaptureMode");
		PrimitiveTypeData uint32 = new(typeof(uint));
		PrimitiveTypeData uintPtr = new(typeof(nuint));

		return
		[
			new(compilerOptions, "CompilerCreateCompilerOptions", [new(compiler, "compiler")]),
			new(resources, "CompilerCreateShaderResources", [new(compiler, "compiler")]),
			new(resources, "CompilerCreateShaderResourcesForActiveVariables", [new(compiler, "compiler"), new(set, "active")]),
			new(set, "CompilerGetActiveInterfaceVariables", [new(compiler, "compiler")]),
			new(constant, "CompilerGetConstantHandle", [new(compiler, "compiler"), new(uint32, "id")]),
			new(type, "CompilerGetTypeHandle", [new(compiler, "compiler"), new(uint32, "id")]),
			new(context, "ContextCreate", []),
			new(compiler, "ContextCreateCompiler", [new(context, "context"), new(backend, "backend"), new(parsedIr, "parsed_ir"), new(captureMode, "mode")]),
			new(parsedIr, "ContextParseSpirv", [new(context, "context"), new(new PointerTypeData(uint32), "spirv"), new(uintPtr, "word_count")]),
		];
	}

	private static IEnumerable<MethodInfo> GetAllMethods(Type type)
	{
		foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
		{
			if (method.IsSpecialName || method.DeclaringType != type || method.IsGenericMethod)
			{
				continue;
			}

			if (method.Name is "ContextSetErrorCallback")
			{
				continue; // Skip the only method with a function pointer
			}

			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Any(p => p.ParameterType.IsByRef))
			{
				// Silk.NET generates a combinatoral explosion of overloads for ref parameters.
				continue;
			}

			yield return method;
		}
	}
}
