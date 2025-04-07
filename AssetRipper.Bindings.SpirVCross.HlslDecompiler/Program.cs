using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using System.Runtime.InteropServices;

namespace AssetRipper.Bindings.SpirVCross.HlslDecompiler;

internal static class Program
{
	//https://github.com/KhronosGroup/SPIRV-Cross?tab=readme-ov-file#using-the-c-api-wrapper
	static unsafe void Main(string[] args)
	{
		Arguments? arguments = Arguments.Parse(args);
		if (arguments == null)
		{
			return;
		}
		string path = arguments.Path;
		byte[] data = File.ReadAllBytes(path);
		ReadOnlySpan<uint> words = MemoryMarshal.Cast<byte, uint>(data);
		List<(string, string)> pendingRenames = [];
		SpirVContext context = SpirVContext.Create();
		try
		{
			SpirVParsedIR ir = context.ParseSpirV(words);

			SpirVCompiler compiler = context.CreateCompiler(Backend.Hlsl, ir, CaptureMode.TakeOwnership);

			SpirVCompilerOptions options = compiler.CreateOptions();
			options.SetOption(CompilerOption.HlslShaderModel, 40); // HLSL shader model 4.0, ie DX10
			options.SetOption(CompilerOption.ForceTemporary, arguments.ForceTemporary);

			compiler.InstallOptions(options);

			compiler.BuildCombinedImageSamplers();

			SpirVResources resources = compiler.CreateShaderResources();

			SpirVReflectedResourceList uniformBufferList = resources.GetResourceListForType(ResourceType.UniformBuffer);
			SpirVReflectedResourceList storageBuffer = resources.GetResourceListForType(ResourceType.StorageBuffer);
			SpirVReflectedResourceList stageInput = resources.GetResourceListForType(ResourceType.StageInput);
			SpirVReflectedResourceList stageOutput = resources.GetResourceListForType(ResourceType.StageOutput);
			SpirVReflectedResourceList subpassInput = resources.GetResourceListForType(ResourceType.SubpassInput);
			SpirVReflectedResourceList storageImage = resources.GetResourceListForType(ResourceType.StorageImage);
			SpirVReflectedResourceList sampleImage = resources.GetResourceListForType(ResourceType.SampledImage);
			SpirVReflectedResourceList atomicCounter = resources.GetResourceListForType(ResourceType.AtomicCounter);
			SpirVReflectedResourceList pushConstant = resources.GetResourceListForType(ResourceType.PushConstant);
			SpirVReflectedResourceList separateImage = resources.GetResourceListForType(ResourceType.SeparateImage);
			SpirVReflectedResourceList separateSamplers = resources.GetResourceListForType(ResourceType.SeparateSamplers);
			SpirVReflectedResourceList accelerationStructure = resources.GetResourceListForType(ResourceType.AccelerationStructure);
			SpirVReflectedResourceList shaderRecordBuffer = resources.GetResourceListForType(ResourceType.ShaderRecordBuffer);

			string? entryPointName = arguments.EntryPointName ?? compiler.ExecutionModel switch
			{
				ExecutionModel.Vertex => "vert",
				ExecutionModel.Geometry => "geom",
				ExecutionModel.Fragment => "frag",
				_ => null,
			};
			if (!string.IsNullOrEmpty(entryPointName))
			{
				const string defaultLine = "SPIRV_Cross_Output main(SPIRV_Cross_Input stage_input)";
				string newLine = $"SPIRV_Cross_Output {entryPointName}(SPIRV_Cross_Input stage_input)";
				pendingRenames.Add((defaultLine, newLine));
			}

			pendingRenames.Add(("SPIRV_Cross_Input", $"{compiler.ExecutionModel.ToCapitalizedName()}_Stage_Input"));
			pendingRenames.Add(("SPIRV_Cross_Output", $"{compiler.ExecutionModel.ToCapitalizedName()}_Stage_Output"));

			if (uniformBufferList.Count == 1)
			{
				SpirVReflectedResource uniformBuffer = uniformBufferList[0];
				SpirVType type = uniformBuffer.Type.BaseType;

				// If the type is unnamed, the compiler will be more cautious about member names.
				// We want "{bufferName}_{memberName}", but the compiler might use something like "{bufferName}_1_{memberName}".
				type.Name = compiler.ExecutionModel.ToLongName() + "_uniform_buffer_type";

				if (arguments.UniformBufferNames is not null
					&& type.NumMemberTypes == arguments.UniformBufferNames.Length)
				{
					string bufferName = RandomName('b');
					uniformBuffer.VariableName = bufferName;

					for (int i = 0; i < arguments.UniformBufferNames.Length; i++)
					{
						string memberName = RandomName('m');
						string realName = arguments.UniformBufferNames[i];
						type.SetMemberName((uint)i, memberName);

						pendingRenames.Add(($"{bufferName}_{memberName}", realName));
					}
				}
			}

			if (arguments.InputNames is not null
				&& stageInput.Count == arguments.InputNames.Length)
			{
				for (int i = 0; i < stageInput.Count; i++)
				{
					SpirVReflectedResource input = stageInput[i];
					string inputName = RandomName('i');
					string realName = arguments.InputNames[input.Location];
					input.VariableName = inputName;
					pendingRenames.Add((inputName, realName));
				}
			}
			else
			{
				string prefix = compiler.ExecutionModel.ToLongName() + "_input_";
				for (int i = 0; i < stageInput.Count; i++)
				{
					SpirVReflectedResource input = stageInput[i];
					string inputName = RandomName('i');
					string realName = $"{prefix}{input.Location}";
					input.VariableName = inputName;
					pendingRenames.Add((inputName, realName));
				}
			}

			if (arguments.OutputNames is not null
				&& stageOutput.Count == arguments.OutputNames.Length)
			{
				for (int i = 0; i < stageOutput.Count; i++)
				{
					SpirVReflectedResource output = stageOutput[i];
					string outputName = RandomName('o');
					string realName = arguments.OutputNames[output.Location];
					output.VariableName = outputName;
					pendingRenames.Add((outputName, realName));
				}
			}
			else
			{
				string prefix = compiler.ExecutionModel.ToLongName() + "_output_";
				for (int i = 0; i < stageOutput.Count; i++)
				{
					SpirVReflectedResource output = stageOutput[i];
					string outputName = RandomName('o');
					string realName = $"{prefix}{i}";
					output.VariableName = outputName;
					pendingRenames.Add((outputName, realName));
				}
			}

			if (arguments.SamplerNames is not null
				&& sampleImage.Count == arguments.SamplerNames.Length)
			{
				for (int i = 0; i < arguments.SamplerNames.Length; i++)
				{
					SpirVReflectedResource sampler = sampleImage[i];
					string samplerName = RandomName('s');
					string realName = arguments.SamplerNames[i];
					sampler.VariableName = samplerName;
					// https://docs.unity3d.com/Manual/SL-SamplerStates.html
					pendingRenames.Add(($"_{samplerName}_sampler", $"sampler{realName}"));
					pendingRenames.Add((samplerName, realName));
				}
			}

			string unnamedPrefix = arguments.UnnamedPrefix ?? compiler.ExecutionModel.ToLongName() + "_unnamed_";
			for (int id = (int)compiler.CurrentIdBound - 1; id >= 0; id--)
			{
				string? name = compiler.GetName((uint)id);
				if (string.IsNullOrEmpty(name))
				{
					compiler.SetName((uint)id, $"{unnamedPrefix}{id}");
				}
			}

			string? result = compiler.Compile();

			foreach ((string oldName, string newName) in pendingRenames)
			{
				result = result?.Replace(oldName, newName);
			}

			Console.WriteLine(result);
		}
		finally
		{
			context.Destroy();
		}
	}

	private static string RandomName(char prefix)
	{
		return $"{prefix}{RandomGuidString()}";
	}

	/// <summary>
	/// Generates a random GUID string.
	/// </summary>
	/// <returns>A 32 character string with no hyphens, underscores, nor whitespace.</returns>
	private static string RandomGuidString()
	{
		return Guid.NewGuid().ToString("N");
	}
}
