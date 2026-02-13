using AssetRipper.Text.SourceGeneration;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace AssetRipper.Bindings.SpirVCross.Generator;

public partial class BindingsGenerator
{
	private static void GenerateMainMethods(IncrementalGeneratorPostInitializationContext context, List<MethodData> methods)
	{
		using StringWriter stringWriter = new() { NewLine = "\n" };
		using IndentedTextWriter writer = IndentedTextWriterFactory.Create(stringWriter);

		writer.WriteGeneratedCodeWarning();
		writer.WriteLineNoTabs();
		writer.WriteFileScopedNamespace(Namespace);
		writer.WriteLineNoTabs();
		writer.WriteLine($"public static unsafe partial class {ClassName}");
		using (new CurlyBrackets(writer))
		{
			foreach (MethodData method in methods)
			{
				writer.Write($"public static {method.ReturnType.FullName} {method.Name}(");
				IReadOnlyList<ParameterData> parameters = method.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterData parameter = parameters[i];
					writer.Write(parameter.Type.FullName);
					writer.Write($" {parameter.Name}");
					if (i < parameters.Count - 1)
					{
						writer.Write(", ");
					}
				}
				writer.WriteLine(')');
				using (new CurlyBrackets(writer))
				{
					if (!method.ReturnType.IsVoid)
					{
						writer.Write("return ");
					}
					writer.Write($"{NativeMethodsGlobalName}.{method.Name}(");
					for (int i = 0; i < parameters.Count; i++)
					{
						ParameterData parameter = parameters[i];
						writer.Write($"{parameter.Name}");
						if (i < parameters.Count - 1)
						{
							writer.Write(", ");
						}
					}
					writer.WriteLine(");");
				}
				writer.WriteLineNoTabs();
			}
		}

		context.AddSource($"{ClassName}.Methods.cs", stringWriter.ToString());
	}
}
