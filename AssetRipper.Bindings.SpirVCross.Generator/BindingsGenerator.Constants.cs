using AssetRipper.Text.SourceGeneration;
using Microsoft.CodeAnalysis;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using System.CodeDom.Compiler;
using System.Reflection;

namespace AssetRipper.Bindings.SpirVCross.Generator;

public partial class BindingsGenerator
{
	private static void GenerateConstants(IncrementalGeneratorPostInitializationContext context)
	{
		using StringWriter stringWriter = new() { NewLine = "\n" };
		using IndentedTextWriter writer = IndentedTextWriterFactory.Create(stringWriter);

		writer.WriteGeneratedCodeWarning();
		writer.WriteLineNoTabs();
		writer.WriteFileScopedNamespace(Namespace);
		writer.WriteLineNoTabs();
		writer.WriteLine($"public static partial class {ClassName}");
		using (new CurlyBrackets(writer))
		{
			foreach (Type type in (ReadOnlySpan<Type>)[typeof(Cross), typeof(Spv)])
			{
				foreach (FieldInfo field in type.GetFields())
				{
					if (field.IsLiteral)
					{
						writer.WriteLine($"public const global::{field.FieldType.FullName} {field.Name} = global::{type.FullName}.{field.Name};");
					}
				}
			}
		}

		context.AddSource($"{ClassName}.Constants.cs", stringWriter.ToString());
	}
}
