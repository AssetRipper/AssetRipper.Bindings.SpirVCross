using AssetRipper.Text.SourceGeneration;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace AssetRipper.Bindings.SpirVCross.Generator;

public partial class BindingsGenerator
{
	private static void GenerateStructMethods(IncrementalGeneratorPostInitializationContext context, List<MethodData> methods, HashSet<GeneratedTypeData> generatedStructs)
	{
		List<GeneratedTypeData> generatedStructList = generatedStructs.OrderByDescending(x => x.Name.Length).ToList();
		Dictionary<GeneratedTypeData, List<MethodData>> generatedStructDictionary = generatedStructList.ToDictionary(t => t, t => new List<MethodData>());
		for (int i = methods.Count - 1; i >= 0; i--)
		{
			MethodData method = methods[i];
			foreach (GeneratedTypeData generatedStruct in generatedStructList)
			{
				if (method.Name.StartsWith(generatedStruct.Name, StringComparison.Ordinal))
				{
					generatedStructDictionary[generatedStruct].Add(method);
					methods.RemoveAt(i);
					break;
				}
			}
		}

		foreach ((GeneratedTypeData generatedStruct, List<MethodData> methodsForStruct) in generatedStructDictionary)
		{
			if (methodsForStruct.Count == 0)
			{
				continue;
			}

			using StringWriter stringWriter = new() { NewLine = "\n" };
			using IndentedTextWriter writer = IndentedTextWriterFactory.Create(stringWriter);

			writer.WriteGeneratedCodeWarning();
			writer.WriteLineNoTabs();
			writer.WriteFileScopedNamespace(generatedStruct.Namespace);
			writer.WriteLineNoTabs();

			writer.WriteLine($"unsafe partial struct {generatedStruct.Name}");
			using (new CurlyBrackets(writer))
			{
				for (int methodIndex = methodsForStruct.Count - 1; methodIndex >= 0; methodIndex--)
				{
					MethodData method = methodsForStruct[methodIndex];
					string name = method.Name[generatedStruct.Name.Length..];

					bool firstParameterIsStruct = method.Parameters.Count > 0 && method.Parameters[0].Type == generatedStruct;
					bool firstParameterIsStructPointer = !firstParameterIsStruct && method.Parameters.Count > 0 && method.Parameters[0].Type == new PointerTypeData(generatedStruct);
					bool isInstance = firstParameterIsStruct || firstParameterIsStructPointer;
					bool isStatic = !isInstance;

					writer.Write("public ");
					if (isStatic)
					{
						writer.Write("static ");
					}
					else if (firstParameterIsStruct)
					{
						writer.Write("readonly ");
					}
					writer.Write(method.ReturnType.FullName);
					writer.Write(' ');
					writer.Write(name);
					writer.Write('(');
					int parameterStartIndex = isInstance ? 1 : 0;
					for (int i = parameterStartIndex; i < method.Parameters.Count; i++)
					{
						ParameterData parameter = method.Parameters[i];
						writer.Write(parameter.Type.FullName);
						writer.Write(' ');
						writer.Write(parameter.Name);
						if (i < method.Parameters.Count - 1)
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
						writer.Write(NativeMethodsGlobalName);
						writer.Write('.');
						writer.Write(method.Name);
						writer.Write('(');
						if (isInstance)
						{
							if (firstParameterIsStruct)
							{
								writer.Write("this");
							}
							else
							{
								writer.Write('(');
								writer.Write(generatedStruct.FullName);
								writer.Write("*)");
								writer.Write("global::System.Runtime.CompilerServices.Unsafe.AsPointer(ref this)");
							}
							if (method.Parameters.Count > 1)
							{
								writer.Write(", ");
							}
						}
						for (int i = parameterStartIndex; i < method.Parameters.Count; i++)
						{
							ParameterData parameter = method.Parameters[i];
							writer.Write(parameter.Name);
							if (i < method.Parameters.Count - 1)
							{
								writer.Write(", ");
							}
						}
						writer.WriteLine(");");
					}
				}
			}

			context.AddSource($"{generatedStruct.Name}.Methods.cs", stringWriter.ToString());
		}
	}
}
