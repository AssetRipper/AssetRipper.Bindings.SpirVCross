using Ookii.CommandLine;
using System.ComponentModel;

namespace AssetRipper.Bindings.SpirVCross.HlslDecompiler;

[GeneratedParser]
[ParseOptions(IsPosix = true)]
partial class Arguments
{
	[CommandLineArgument(IsPositional = true)]
	[Description("The path to the SPIR-V file.")]
	public required string Path { get; init; }

	[CommandLineArgument]
	[Description("If true, temporary variables will not be inlined.")]
	public bool ForceTemporary { get; set; }

	[CommandLineArgument]
	[Description("The name of the main function.")]
	public string? EntryPointName { get; set; }

	[CommandLineArgument]
	[Description("The set of uniform buffer names.")]
	public string[]? UniformBufferNames { get; set; }

	[CommandLineArgument]
	[Description("The set of input names.")]
	public string[]? InputNames { get; set; }

	[CommandLineArgument]
	[Description("The set of output names.")]
	public string[]? OutputNames { get; set; }

	[CommandLineArgument]
	[Description("The set of sampler names.")]
	public string[]? SamplerNames { get; set; }

	[CommandLineArgument]
	[Description("The string to use when prefixing entities that haven't been named.")]
	public string? UnnamedPrefix { get; set; }
}
