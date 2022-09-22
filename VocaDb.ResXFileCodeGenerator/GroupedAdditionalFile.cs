using Microsoft.CodeAnalysis;
namespace VocaDb.ResXFileCodeGenerator;

public readonly record struct GroupedAdditionalFile
{
	public AdditionalTextWithHash MainFile { get; }
	public IReadOnlyList<AdditionalTextWithHash> SubFiles { get; }

	public GroupedAdditionalFile(AdditionalTextWithHash mainFile, IReadOnlyList<AdditionalTextWithHash> subFiles)
	{
		MainFile = mainFile;
		SubFiles = subFiles.OrderBy(x => x.File.Path, StringComparer.Ordinal).ToArray();
	}

	public bool Equals(GroupedAdditionalFile other)
	{
		return MainFile.Equals(other.MainFile) && SubFiles.SequenceEqual(other.SubFiles);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hashCode = MainFile.GetHashCode();

			foreach (var additionalText in SubFiles)
			{
				hashCode = (hashCode * 397) ^ additionalText.GetHashCode();
			}

			return hashCode;
		}
	}

	public override string ToString()
	{
		return $"{nameof(MainFile)}: {MainFile}, {nameof(SubFiles)}: {string.Join("; ", SubFiles ?? Array.Empty<AdditionalTextWithHash>())}";
	}
}
