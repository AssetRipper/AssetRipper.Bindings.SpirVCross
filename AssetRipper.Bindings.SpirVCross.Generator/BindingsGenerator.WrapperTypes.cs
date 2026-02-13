using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross.Generator;

public partial class BindingsGenerator
{
	private static void GenerateWrapperTypes(IncrementalGeneratorPostInitializationContext context, List<(SilkNetTypeData, GeneratedTypeData)> opaqueTypes, HashSet<GeneratedTypeData> generatedStructs, Dictionary<TypeData, TypeData> replacements, out HashSet<GeneratedTypeData> wrapperStructs)
	{
		wrapperStructs = [];
		foreach ((SilkNetTypeData silkNetType, GeneratedTypeData opaqueType) in opaqueTypes)
		{
			Debug.Assert(opaqueType.Name.StartsWith("Opaque", StringComparison.Ordinal));
			string name = opaqueType.Name["Opaque".Length..];
			context.AddSource($"{name}.cs", $$"""
			namespace {{Namespace}}
			{
				public unsafe partial struct {{name}}
				{
					public static implicit operator {{opaqueType.FullName}}*({{name}} value) => value.Handle;
					public static implicit operator {{silkNetType.FullName}}*({{name}} value) => ({{silkNetType.FullName}}*)value.Handle;
				}
			}
			""");
			GeneratedTypeData generatedType = new(Namespace, name);
			replacements.Add(new PointerTypeData(silkNetType), generatedType);
			replacements.Add(new PointerTypeData(opaqueType), generatedType);
			generatedStructs.Add(generatedType);
			wrapperStructs.Add(generatedType);
		}
	}
}
