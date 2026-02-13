using AssetRipper.Text.SourceGeneration;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Reflection;

namespace AssetRipper.Bindings.SpirVCross.Generator;

public partial class BindingsGenerator
{
	private static void GenerateTypes(
		IncrementalGeneratorPostInitializationContext context,
		List<MethodData> methods,
		out HashSet<GeneratedTypeData> generatedStructs,
		out List<(SilkNetTypeData, GeneratedTypeData)> needsMembers,
		out List<(SilkNetTypeData, GeneratedTypeData)> opaqueTypes,
		out Dictionary<TypeData, TypeData> replacements)
	{
		generatedStructs = [];
		HashSet<TypeData> rootTypes = [];
		foreach (MethodData method in methods)
		{
			rootTypes.Add(method.ReturnType.RootType);
			foreach (ParameterData parameter in method.Parameters)
			{
				rootTypes.Add(parameter.Type.RootType);
			}
		}
		Queue<TypeData> typesToProcess = new(rootTypes);
		replacements = new(rootTypes.Count * 2);
		needsMembers = [];
		opaqueTypes = [];
		while (typesToProcess.Count > 0)
		{
			if (typesToProcess.Dequeue() is not SilkNetTypeData silkNetType)
			{
				continue;
			}

			Type systemType = silkNetType.Type;
			string name = silkNetType.Type.Name;
			if (name is "CrossType")
			{
				name = "Type"; // Undo Silk.NET renaming
			}
			else if (name is "ParsedIr")
			{
				name = "ParsedIR"; // Fix capitalization
			}

			if (systemType.IsEnum)
			{
				Type underlyingType = Enum.GetUnderlyingType(systemType);
				string underlyingTypeName = $"global::{underlyingType.FullName}";

				using StringWriter stringWriter = new() { NewLine = "\n" };
				using IndentedTextWriter writer = IndentedTextWriterFactory.Create(stringWriter);

				writer.WriteGeneratedCodeWarning();
				writer.WriteLineNoTabs();
				writer.WriteFileScopedNamespace(Namespace);
				writer.WriteLineNoTabs();
				writer.WriteLine($"public enum {name} : {underlyingTypeName}");
				using (new CurlyBrackets(writer))
				{
					foreach (FieldInfo field in systemType.GetFields(BindingFlags.Public | BindingFlags.Static))
					{
						writer.WriteLine($"{field.Name} = {silkNetType.FullName}.{field.Name},");
					}
				}

				context.AddSource($"{name}.cs", stringWriter.ToString());
				replacements.Add(silkNetType, new GeneratedTypeData(Namespace, name));
			}
			else if (!systemType.IsValueType)
			{
				// Should never happen
			}
			else if (systemType.GetFields(BindingFlags.Public | BindingFlags.Instance).Length is 0)
			{
				// Opaque struct, we can replace it with an empty struct.
				context.AddSource($"Opaque{name}.cs", $$"""
				namespace {{LowLevelNamespace}}
				{
					public partial struct Opaque{{name}}
					{
					}
				}
				""");
				GeneratedTypeData opaqueType = new(LowLevelNamespace, $"Opaque{name}");
				replacements.Add(silkNetType, opaqueType);
				opaqueTypes.Add((silkNetType, opaqueType));
				generatedStructs.Add(opaqueType);
			}
			else
			{
				// Regular struct, we will need to copy its fields, but not yet.
				context.AddSource($"{name}.cs", $$"""
				namespace {{Namespace}}
				{
					public partial struct {{name}}
					{
					}
				}
				""");
				GeneratedTypeData generatedType = new(Namespace, name);
				replacements.Add(silkNetType, generatedType);

				foreach (FieldInfo field in systemType.GetFields(BindingFlags.Public | BindingFlags.Instance))
				{
					TypeData fieldRootType = TypeData.FromRuntimeType(field.FieldType).RootType;
					if (rootTypes.Add(fieldRootType))
					{
						typesToProcess.Enqueue(fieldRootType);
					}
				}
				needsMembers.Add((silkNetType, generatedType));
				generatedStructs.Add(generatedType);
			}
		}
	}
}
