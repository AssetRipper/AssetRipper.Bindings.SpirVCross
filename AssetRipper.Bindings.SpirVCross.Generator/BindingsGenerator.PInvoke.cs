using AssetRipper.Text.SourceGeneration;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace AssetRipper.Bindings.SpirVCross.Generator;

public partial class BindingsGenerator
{
	private static void GeneratePInvokeMethods(IncrementalGeneratorPostInitializationContext context, List<MethodData> methods, Dictionary<TypeData, TypeData> replacements)
	{
		using StringWriter stringWriter = new() { NewLine = "\n" };
		using IndentedTextWriter writer = IndentedTextWriterFactory.Create(stringWriter);

		writer.WriteGeneratedCodeWarning();
		writer.WriteLineNoTabs();
		writer.WriteFileScopedNamespace(LowLevelNamespace);
		writer.WriteLineNoTabs();
		writer.WriteLine($"public static unsafe partial class {PInvokeClassName}");
		using (new CurlyBrackets(writer))
		{
			for (int methodIndex = 0; methodIndex < methods.Count; methodIndex++)
			{
				MethodData method = methods[methodIndex];
				MethodData replacedMethod = MethodData.Replace(method, replacements);
				if (replacedMethod.Name.EndsWith("Init2", StringComparison.Ordinal))
				{
					string newName = replacedMethod.Name.Substring(0, replacedMethod.Name.Length - "Init2".Length) + "2Init";
					replacedMethod = new(replacedMethod.ReturnType, newName, replacedMethod.Parameters);
				}
				else if (replacedMethod.Name is "CompilerOptionsSetUint")
				{
					replacedMethod = new(replacedMethod.ReturnType, "CompilerOptionsSetUInt", replacedMethod.Parameters);
				}
				methods[methodIndex] = replacedMethod; // Update the method in-place

				writer.Write($"public static {replacedMethod.ReturnType.FullName} {replacedMethod.Name}(");
				writer.Write(replacedMethod.ParametersStringWithTypes);
				writer.WriteLine(')');
				using (new CurlyBrackets(writer))
				{
					if (!replacedMethod.ReturnType.IsVoid)
					{
						writer.Write("return ");
						TypeData replacedReturnType = replacedMethod.ReturnType;
						TypeData originalReturnType = method.ReturnType;
						if (originalReturnType != replacedReturnType)
						{
							writer.Write($"({replacedReturnType.FullName})");
						}
					}

					writer.Write($"{PInvokeGlobalName}.ApiInstance.{method.Name}(");
					IReadOnlyList<ParameterData> originalParameters = method.Parameters;
					for (int i = 0; i < originalParameters.Count; i++)
					{
						ParameterData originalParameter = originalParameters[i];
						ParameterData replacedParameter = replacedMethod.Parameters[i];
						TypeData originalParameterType = originalParameter.Type;
						TypeData replacedParameterType = replacedParameter.Type;

						if (originalParameterType != replacedParameterType)
						{
							writer.Write($"({originalParameterType.FullName})");
						}
						writer.Write(replacedParameter.Name);
						if (i < originalParameters.Count - 1)
						{
							writer.Write(", ");
						}
					}
					writer.WriteLine(");");
				}
				writer.WriteLineNoTabs();
			}
		}

		context.AddSource($"{PInvokeClassName}.Methods.cs", stringWriter.ToString());
	}
}
