namespace AssetRipper.Bindings.SpirVCross.Exceptions;

public sealed class InvalidArgumentException : ArgumentException
{
	internal InvalidArgumentException(string message) : base(message)
	{
	}
}
