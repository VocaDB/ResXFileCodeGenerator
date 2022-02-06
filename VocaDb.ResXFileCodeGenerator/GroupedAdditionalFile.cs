using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public class GroupedAdditionalFile : IEquatable<GroupedAdditionalFile>
{
	public AdditionalText Mainfile { get; }
	public IReadOnlyList<AdditionalText> Subfiles { get; init; }

	public GroupedAdditionalFile(AdditionalText mainfile, IReadOnlyList<AdditionalText> subfiles)
	{
		Mainfile = mainfile;
		Subfiles = subfiles.OrderBy(x=>x.Path, StringComparer.Ordinal).ToArray();
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

		return Mainfile.Path.Equals(other.Mainfile.Path) && Subfiles.Select(x=>x.Path).SequenceEqual(other.Subfiles.Select(x=>x.Path));
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
			var hashCode = Mainfile.Path.GetHashCode();
			foreach (var additionalText in Subfiles)
			{
				hashCode = (hashCode * 397) ^ additionalText.Path.GetHashCode();
			}
			return hashCode;
		}
	}

	public static bool operator ==(GroupedAdditionalFile? left, GroupedAdditionalFile? right) => Equals(left, right);

	public static bool operator !=(GroupedAdditionalFile? left, GroupedAdditionalFile? right) => !Equals(left, right);
}
