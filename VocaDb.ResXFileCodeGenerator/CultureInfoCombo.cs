using System.Globalization;
using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public class CultureInfoCombo : IEquatable<CultureInfoCombo>
{
	//order by length desc, so that da-DK comes before da, meaning that it HashSet<int> already doesn't contain da-DK when we process it
	public CultureInfoCombo(IReadOnlyList<AdditionalText> subfiles) =>
		CultureInfos = subfiles.Select(x => (Path.GetExtension(Path.GetFileNameWithoutExtension(x.Path)).TrimStart('.'), y: x))
			.OrderByDescending(x => x.Item1.Length).ThenBy(y => y.Item1).ToList();

	public CultureInfoCombo() => CultureInfos = Array.Empty<(string, AdditionalText)>();

	public IReadOnlyList<(string Iso, AdditionalText File)> CultureInfos { get; init; }

	public IReadOnlyList<(string Name, int LCID, AdditionalText File)> GetDefinedLanguages() => CultureInfos
		.Select(x => (x.File, new CultureInfo(x.Iso)))
		.Select(x => (Name: x.Item2.Name.Replace('-', '_'), x.Item2.LCID, x.File)).ToList();

	public bool Equals(CultureInfoCombo? other)
	{
		if (ReferenceEquals(null, other))
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return CultureInfos.Select(x=>x.Iso).SequenceEqual(other.CultureInfos.Select(x=>x.Iso));
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

		return Equals((CultureInfoCombo)obj);
	}

	public override int GetHashCode() => CultureInfos.Count.GetHashCode();

	public static bool operator ==(CultureInfoCombo? left, CultureInfoCombo? right) => Equals(left, right);

	public static bool operator !=(CultureInfoCombo? left, CultureInfoCombo? right) => !Equals(left, right);
}
