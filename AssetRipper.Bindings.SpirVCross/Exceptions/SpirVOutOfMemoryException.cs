namespace AssetRipper.Bindings.SpirVCross.Exceptions;

public sealed class SpirVOutOfMemoryException : OutOfMemoryException
{
	internal SpirVOutOfMemoryException(string message) : base(message)
	{
	}
}
