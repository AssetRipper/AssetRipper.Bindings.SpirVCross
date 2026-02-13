using System.Reflection;

namespace AssetRipper.Bindings.SpirVCross.Generator;

internal readonly struct ParameterData
{
	public TypeData Type { get; }
	public string Name { get; }

	public ParameterData(TypeData type, string name)
	{
		Type = type;
		Name = name;
	}

	public static ParameterData FromRuntimeParameter(ParameterInfo parameter)
	{
		return new ParameterData(TypeData.FromRuntimeType(parameter.ParameterType), parameter.Name);
	}

	public static ParameterData Replace(ParameterData original, Dictionary<TypeData, TypeData> replacements)
	{
		return new ParameterData(TypeData.Replace(original.Type, replacements), original.Name);
	}

	public static bool TryReplace(ParameterData original, Dictionary<TypeData, TypeData> replacements, out ParameterData replacement)
	{
		if (TypeData.TryReplace(original.Type, replacements, out TypeData? newType))
		{
			replacement = new ParameterData(newType, original.Name);
			return true;
		}
		replacement = original;
		return false;
	}
}
