using AssetRipper.Bindings.SpirVCross.Exceptions;
using System.Diagnostics;

namespace AssetRipper.Bindings.SpirVCross;

public static class ResultExtensions
{
	[StackTraceHidden]
	public static unsafe void ThrowIfError(this Result result, Context* context)
	{
		if (result != Result.Success)
		{
			string? errorString = context != null ? SpirVCrossNative.ContextGetLastErrorStringS(context) : null;
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

	[StackTraceHidden]
	public static void ThrowIfError(this Result result, SpirVContext context)
	{
		if (result != Result.Success)
		{
			string? errorString = context.LastErrorString;
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
