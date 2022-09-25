using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public readonly record struct AdditionalTextWithHash(AdditionalText File, Guid Hash)
{
	public bool Equals(AdditionalTextWithHash other)
	{
		return File.Path.Equals(other.File.Path) && Hash.Equals(other.Hash);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			return (File.GetHashCode() * 397) ^ Hash.GetHashCode();
		}
	}

	public override string ToString()
	{
		return $"{nameof(File)}: {File?.Path}, {nameof(Hash)}: {Hash}";
	}
}
