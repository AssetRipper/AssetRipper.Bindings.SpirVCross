namespace AssetRipper.Bindings.SpirVCross.HlslDecompiler;

internal static class ExecutionModelExtensions
{
	public static string ToShortName(this ExecutionModel value) => value switch
	{
		ExecutionModel.Vertex => "vert",
		ExecutionModel.Geometry => "geom",
		ExecutionModel.Fragment => "frag",
		_ => "unkn",
	};

	public static string ToLongName(this ExecutionModel value) => value switch
	{
		ExecutionModel.Vertex => "vertex",
		ExecutionModel.Geometry => "geometry",
		ExecutionModel.Fragment => "fragment",
		_ => "unknown",
	};

	public static string ToCapitalizedName(this ExecutionModel value) => value switch
	{
		ExecutionModel.Vertex => "Vertex",
		ExecutionModel.Geometry => "Geometry",
		ExecutionModel.Fragment => "Fragment",
		_ => "Unknown",
	};
}
