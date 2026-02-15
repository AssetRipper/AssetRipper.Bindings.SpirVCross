using AssetRipper.Text.SourceGeneration;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace AssetRipper.Bindings.SpirVCross.Generator;

public partial class BindingsGenerator
{
	private static void GenerateNativeMethods(IncrementalGeneratorPostInitializationContext context, List<MethodData> methods, Dictionary<TypeData, TypeData> replacements)
	{
		using StringWriter stringWriter = new() { NewLine = "\n" };
		using IndentedTextWriter writer = IndentedTextWriterFactory.Create(stringWriter);

		writer.WriteGeneratedCodeWarning();
		writer.WriteLineNoTabs();
		writer.WriteFileScopedNamespace(LowLevelNamespace);
		writer.WriteLineNoTabs();
		writer.WriteLine($"public static unsafe partial class {NativeMethodsClassName}");
		using (new CurlyBrackets(writer))
		{
			for (int methodIndex = 0; methodIndex < methods.Count; methodIndex++)
			{
				MethodData method = methods[methodIndex];
				if (ManuallyImplementedNativeMethods.TryGetValue(method.Name, out MethodData manuallyImplemented))
				{
					methods[methodIndex] = manuallyImplemented; // Update the method in-place
					continue;
				}

				MethodData replacedMethod = MethodData.Replace(method, replacements);
				if (replacedMethod.ReturnType is GeneratedTypeData { Name: "Result" })
				{
					replacedMethod = new MethodData(new PrimitiveTypeData(typeof(void)), replacedMethod.Name, replacedMethod.Parameters);
				}
				else if (ShouldReturnBoolean(method))
				{
					replacedMethod = new MethodData(new PrimitiveTypeData(typeof(bool)), replacedMethod.Name, replacedMethod.Parameters);
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
						if (method.ReturnType == replacedMethod.ReturnType)
						{
						}
						else if (method.ReturnType is PrimitiveTypeData { IsByte: true } && replacedMethod.ReturnType is PrimitiveTypeData { IsBoolean: true })
						{
							writer.Write("0 != ");
						}
						else
						{
							throw new Exception("This shouldn't happen. We manually implement any methods that return a wrapper type.");
						}
					}

					writer.Write($"{PInvokeGlobalName}.{method.Name}(");
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
					writer.WriteLine(')');
					if (method.ReturnType is GeneratedTypeData { Name: "Result" })
					{
						writer.Write(".ThrowIfError(");
						writer.Write(replacedMethod.Parameters[0].Name);
						if (replacedMethod.Parameters[0].Type is not GeneratedTypeData { Name: "Context" })
						{
							writer.Write(".Context");
						}
						writer.Write(')');
					}
					writer.WriteLine(';');
				}
				writer.WriteLineNoTabs();
			}
		}

		context.AddSource($"{NativeMethodsClassName}.Methods.cs", stringWriter.ToString());

		static bool ShouldReturnBoolean(MethodData method)
		{
			if (method.ReturnType is not PrimitiveTypeData { IsByte: true })
			{
				return false;
			}

			int hasIndex = method.Name.IndexOf("Has", StringComparison.Ordinal);
			if (hasIndex >= 0)
			{
				return method.Name.Length > hasIndex + 3 && char.IsUpper(method.Name[hasIndex + 3]);
			}

			int isIndex = method.Name.IndexOf("Is", StringComparison.Ordinal);
			if (isIndex >= 0)
			{
				return method.Name.Length > isIndex + 2 && char.IsUpper(method.Name[isIndex + 2]);
			}

			return false;
		}
	}
}
