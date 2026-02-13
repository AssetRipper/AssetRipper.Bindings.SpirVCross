using System.Diagnostics.CodeAnalysis;

namespace AssetRipper.Bindings.SpirVCross.Generator;

internal abstract record class TypeData
{
	public abstract string FullName { get; }
	public virtual bool IsVoid => false;
	public virtual TypeData RootType => this;

	public static TypeData FromRuntimeType(Type type)
	{
		if (type.IsPointer)
		{
			return new PointerTypeData(FromRuntimeType(type.GetElementType()!));
		}
		if (type.IsArray)
		{
			return new ArrayTypeData(FromRuntimeType(type.GetElementType()!));
		}
		if (type.IsPrimitive || type == typeof(void) || type == typeof(string))
		{
			return new PrimitiveTypeData(type);
		}
		return new SilkNetTypeData(type);
	}

	public static TypeData Replace(TypeData original, Dictionary<TypeData, TypeData> replacements)
	{
		return TryReplace(original, replacements, out TypeData? replacement) ? replacement : original;
	}

	public static bool TryReplace(TypeData original, Dictionary<TypeData, TypeData> replacements, [NotNullWhen(true)] out TypeData? replacement)
	{
		if (replacements.TryGetValue(original, out replacement))
		{
			return true;
		}
		switch (original)
		{
			case PointerTypeData pointer:
				if (TryReplace(pointer.ElementType, replacements, out TypeData? elementReplacement))
				{
					replacement = new PointerTypeData(elementReplacement);
					return true;
				}
				return false;
			case ArrayTypeData:
				// Array element types cannot be changed.
				return false;
			default:
				return false;
		}
	}
}
