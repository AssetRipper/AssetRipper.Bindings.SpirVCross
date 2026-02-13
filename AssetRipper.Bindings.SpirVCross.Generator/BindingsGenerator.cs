using AssetRipper.Text.SourceGeneration;
using Microsoft.CodeAnalysis;
using SGF;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using System.CodeDom.Compiler;
using System.Reflection;

namespace AssetRipper.Bindings.SpirVCross.Generator;

[IncrementalGenerator]
public partial class BindingsGenerator() : IncrementalGenerator(nameof(BindingsGenerator))
{
	private const string Namespace = "AssetRipper.Bindings.SpirVCross";
	private const string ClassName = "SpirVCrossNative";

	public override void OnInitialize(SgfInitializationContext context)
	{
		context.RegisterPostInitializationOutput(GenerateConstantsFile);
		context.RegisterPostInitializationOutput(GenerateMethodsFile);
	}

	private static void GenerateMethodsFile(IncrementalGeneratorPostInitializationContext context)
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
			CopyMethods(writer, typeof(Cross));
		}

		context.AddSource($"{ClassName}.Methods.g.cs", stringWriter.ToString());
	}

	private static void CopyMethods(IndentedTextWriter writer, Type type)
	{
		foreach (MethodInfo method in GetAllMethods(type).OrderBy(m => m.Name))
		{
			if (method.IsSpecialName || method.DeclaringType != type || method.IsGenericMethod)
			{
				continue;
			}

			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Any(p => p.ParameterType.IsByRef))
			{
				// Silk.NET generates a combinatoral explosion of overloads for ref parameters.
				continue;
			}

			writer.WriteComment(method.Name);
			writer.Write($"public static {ToFullName(method.ReturnType)} {method.Name}(");
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameter = parameters[i];
				writer.Write(ToFullName(parameter.ParameterType));
				writer.Write($" {parameter.Name}");
				if (i < parameters.Length - 1)
				{
					writer.Write(", ");
				}
			}
			writer.WriteLine(')');
			using (new CurlyBrackets(writer))
			{
				if (method.ReturnType != typeof(void))
				{
					writer.Write("return ");
				}
				writer.Write($"ApiInstance.{method.Name}(");
				for (int i = 0; i < parameters.Length; i++)
				{
					ParameterInfo parameter = parameters[i];
					writer.Write($"{parameter.Name}");
					if (i < parameters.Length - 1)
					{
						writer.Write(", ");
					}
				}
				writer.WriteLine(");");
			}
			writer.WriteLineNoTabs();
		}
	}

	private static IEnumerable<MethodInfo> GetAllMethods(Type type)
	{
		foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
		{
			if (method.IsSpecialName || method.DeclaringType != type || method.IsGenericMethod)
			{
				continue;
			}

			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Any(p => p.ParameterType.IsByRef))
			{
				// Silk.NET generates a combinatoral explosion of overloads for ref parameters.
				continue;
			}

			yield return method;
		}
	}

	private static void GenerateConstantsFile(IncrementalGeneratorPostInitializationContext context)
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
				CopyConstants(writer, type);
			}
		}

		context.AddSource($"{ClassName}.Constants.g.cs", stringWriter.ToString());
	}

	private static void CopyConstants(IndentedTextWriter writer, Type type)
	{
		foreach (FieldInfo field in type.GetFields())
		{
			if (field.IsLiteral)
			{
				writer.WriteLine($"public const global::{field.FieldType.FullName} {field.Name} = global::{type.FullName}.{field.Name};");
			}
		}
	}

	private static string ToFullName(Type type)
	{
		if (type == typeof(void))
		{
			return "void";
		}
		if (type == typeof(void*))
		{
			return "void*";
		}
		return $"global::{type.FullName}";
	}
}
