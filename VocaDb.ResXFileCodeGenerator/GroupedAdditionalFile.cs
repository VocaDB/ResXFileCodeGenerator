using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public class GroupedAdditionalFile : IEquatable<GroupedAdditionalFile>
{
	public AdditionalText MainFile { get; }
	public IReadOnlyList<AdditionalText> SubFiles { get; init; }

	public GroupedAdditionalFile(AdditionalText mainFile, IReadOnlyList<AdditionalText> subFiles)
	{
		MainFile = mainFile;
		SubFiles = subFiles.OrderBy(x => x.Path, StringComparer.Ordinal).ToArray();
	}

	public bool Equals(GroupedAdditionalFile? other)
	{
		if (ReferenceEquals(null, other))
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return MainFile.Path.Equals(other.MainFile.Path) && SubFiles.Select(x => x.Path).SequenceEqual(other.SubFiles.Select(x => x.Path));
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != this.GetType())
		{
			return false;
		}

		return Equals((GroupedAdditionalFile)obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hashCode = MainFile.Path.GetHashCode();
			foreach (var additionalText in SubFiles)
			{
				hashCode = (hashCode * 397) ^ additionalText.Path.GetHashCode();
			}
			return hashCode;
		}
	}

	public static bool operator ==(GroupedAdditionalFile? left, GroupedAdditionalFile? right) => Equals(left, right);

	public static bool operator !=(GroupedAdditionalFile? left, GroupedAdditionalFile? right) => !Equals(left, right);
}
