using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public static class GroupResxFiles
{
	public static IEnumerable<GroupedAdditionalFile> Group(IReadOnlyList<AdditionalText> allFiles)
	{
		var lookup = new Dictionary<string, AdditionalText>();
		var res = new Dictionary<AdditionalText, List<AdditionalText>>();
		foreach (var file in allFiles)
		{
			var path = file.Path;
			var pathName = Path.GetDirectoryName(path);
			var baseName = Utilities.GetBaseName(path);
			if (Path.GetFileNameWithoutExtension(path) == baseName)
			{
				lookup.Add(pathName + "\\" + baseName, file);
				res.Add(file, new());
			}
		}
		foreach (var file in allFiles)
		{
			var path = file.Path;
			var pathName = Path.GetDirectoryName(path);
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			var baseName = Utilities.GetBaseName(path);
			if (fileNameWithoutExtension == baseName)
				continue;
			//this might happen if a nn.resx file exists without a .resx file
			if (!lookup.TryGetValue(pathName + "\\" + baseName, out var additionalText))
				continue;
			res[additionalText].Add(file);
		}
		//dont care at all HOW it is sorted, just that end result is the same
		foreach (var file in res)
			yield return new (file.Key, file.Value);
	}

	public static IEnumerable<CultureInfoCombo> DetectChildComboes(IReadOnlyList<GroupedAdditionalFile> groupedAdditionalFiles) =>
		groupedAdditionalFiles
			.Select(x => new CultureInfoCombo(x.Subfiles))
			.Distinct();
}
