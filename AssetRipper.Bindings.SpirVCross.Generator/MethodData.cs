using Microsoft.CodeAnalysis;
using System.Reflection;

namespace AssetRipper.Bindings.SpirVCross.Generator;

internal readonly struct MethodData : IComparable<MethodData>
{
	public TypeData ReturnType { get; }
	public string Name { get; }
	public IReadOnlyList<ParameterData> Parameters { get; }
	public string ParametersStringWithTypes => string.Join(", ", Parameters.Select(p => $"{p.Type.FullName} {p.Name}"));
	public string ParametersStringWithoutTypes => string.Join(", ", Parameters.Select(p => p.Name));
	public MethodData(TypeData returnType, string name, IReadOnlyList<ParameterData> parameters)
	{
		ReturnType = returnType;
		Name = name;
		Parameters = parameters;
	}
	public static MethodData FromRuntimeMethod(MethodInfo method)
	{
		return new MethodData(TypeData.FromRuntimeType(method.ReturnType), method.Name, method.GetParameters().Select(ParameterData.FromRuntimeParameter).ToArray());
	}
	public static MethodData Replace(MethodData original, Dictionary<TypeData, TypeData> replacements)
	{
		return new MethodData(TypeData.Replace(original.ReturnType, replacements), original.Name, original.Parameters.Select(p => ParameterData.Replace(p, replacements)).ToArray());
	}

	int IComparable<MethodData>.CompareTo(MethodData other)
	{
		return string.Compare(Name, other.Name, StringComparison.Ordinal);
	}
}
