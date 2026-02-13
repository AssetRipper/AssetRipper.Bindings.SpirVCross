using AssetRipper.Bindings.SpirVCross.Exceptions;
using AssetRipper.Bindings.SpirVCross.LowLevel;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public static class ResultExtensions
{
	[StackTraceHidden]
	public static unsafe void ThrowIfError(this Result result, Context context)
	{
		if (result != Result.Success)
		{
			string? errorString = context.IsNull ? null : NativeMethods.ContextGetLastErrorStringS(context);
			string message = string.IsNullOrEmpty(errorString) ? result.ToString() : errorString;
			throw result switch
			{
				Result.ErrorInvalidSpirv => new InvalidSpirVException(message),
				Result.ErrorUnsupportedSpirv => new UnsupportedSpirVException(message),
				Result.ErrorOutOfMemory => new SpirVOutOfMemoryException(message),
				Result.ErrorInvalidArgument => new InvalidArgumentException(message),
				_ => new Exception(result.ToString())
			};
		}
	}
}
