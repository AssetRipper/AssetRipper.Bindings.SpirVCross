using AssetRipper.Text.SourceGeneration;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Reflection;

namespace AssetRipper.Bindings.SpirVCross.Generator;

public partial class BindingsGenerator
{
	private static void GenerateStructFields(IncrementalGeneratorPostInitializationContext context, List<(SilkNetTypeData, GeneratedTypeData)> needsMembers, Dictionary<TypeData, TypeData> replacements)
	{

		foreach ((SilkNetTypeData silkNetType, GeneratedTypeData generatedType) in needsMembers)
		{
			using StringWriter stringWriter = new() { NewLine = "\n" };
			using IndentedTextWriter writer = IndentedTextWriterFactory.Create(stringWriter);
			writer.WriteGeneratedCodeWarning();
			writer.WriteLineNoTabs();
			writer.WriteFileScopedNamespace(Namespace);
			writer.WriteLineNoTabs();
			writer.WriteLine($"public unsafe partial struct {generatedType.Name}");
			using (new CurlyBrackets(writer))
			{
				FieldInfo[] fields = silkNetType.Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (FieldInfo field in fields)
				{
					TypeData fieldType = TypeData.FromRuntimeType(field.FieldType);
					TypeData replacedFieldType = TypeData.Replace(fieldType, replacements);
					writer.WriteLine($"public {replacedFieldType.FullName} {field.Name};");
				}
				writer.WriteLineNoTabs();

				writer.WriteLine($"public {generatedType.Name}({silkNetType.FullName} value)");
				using (new CurlyBrackets(writer))
				{
					foreach (FieldInfo field in fields)
					{
						TypeData fieldType = TypeData.FromRuntimeType(field.FieldType);
						TypeData replacedFieldType = TypeData.Replace(fieldType, replacements);
						writer.Write($"this.{field.Name} = ");
						if (fieldType != replacedFieldType)
						{
							writer.Write($"({replacedFieldType.FullName})");
						}
						writer.WriteLine($"value.{field.Name};");
					}
				}
				writer.WriteLineNoTabs();

				// Implicit conversion from the original Silk.NET type to the generated type, using the constructor we just defined.
				writer.WriteLine($"public static implicit operator {generatedType.FullName}({silkNetType.FullName} value) => new(value);");
				writer.WriteLineNoTabs();

				// Implicit conversion from the generated type to the original Silk.NET type, using a field-by-field conversion.
				writer.WriteLine($"public static implicit operator {silkNetType.FullName}({generatedType.FullName} value) => new()");
				using (new CurlyBracketsWithSemicolon(writer))
				{
					foreach (FieldInfo field in fields)
					{
						TypeData originalFieldType = TypeData.FromRuntimeType(field.FieldType);
						TypeData replacedFieldType = TypeData.Replace(originalFieldType, replacements);
						writer.Write($"{field.Name} = ");
						if (originalFieldType != replacedFieldType)
						{
							writer.Write($"({originalFieldType.FullName})");
						}
						writer.WriteLine($"value.{field.Name},");
					}
				}
			}
			context.AddSource($"{generatedType.Name}.Members.cs", stringWriter.ToString());
		}
	}
}
