using System.Globalization;

namespace VocaDb.ResXFileCodeGenerator;

public static class Utilities
{
	// Code from: https://github.com/dotnet/ResXResourceManager/blob/0ec11bae232151400a5a8ca7b9835ac063c516d0/src/ResXManager.Model/ResourceManager.cs#L267

	private static bool IsValidLanguageName(string? languageName)
	{
		try
		{
			if (languageName.IsNullOrEmpty())
			{
				return false;
			}

			if (languageName.StartsWith("qps-", StringComparison.Ordinal))
			{
				return true;
			}

			var culture = new CultureInfo(languageName);

			while (!culture.IsNeutralCulture)
			{
				culture = culture.Parent;
			}

			return culture.LCID != 4096;
		}
		catch
		{
			return false;
		}
	}

	// Code from: https://github.com/dotnet/ResXResourceManager/blob/0ec11bae232151400a5a8ca7b9835ac063c516d0/src/ResXManager.Model/ProjectFileExtensions.cs#L77

	public static string GetBaseName(string filePath)
	{
		var name = Path.GetFileNameWithoutExtension(filePath);
		var innerExtension = Path.GetExtension(name);
		var languageName = innerExtension.TrimStart('.');

		return IsValidLanguageName(languageName) ? Path.GetFileNameWithoutExtension(name) : name;
	}

	// Code from: https://github.com/dotnet/ResXResourceManager/blob/c8b5798d760f202a1842a74191e6010c6e8bbbc0/src/ResXManager.VSIX/Visuals/MoveToResourceViewModel.cs#L120

	public static string GetLocalNamespace(string? resxPath, string? targetPath, string? projectPath,
		string? rootNamespace)
	{
		try
		{
			if (resxPath is null)
			{
				return string.Empty;
			}

			var resxFolder = Path.GetDirectoryName(resxPath);
			var projectFolder = Path.GetDirectoryName(projectPath);
			rootNamespace ??= string.Empty;

			if (resxFolder is null || projectFolder is null)
			{
				return string.Empty;
			}

			var localNamespace = rootNamespace;

			if (!string.IsNullOrWhiteSpace(targetPath))
			{
				var ns = Path.GetDirectoryName(targetPath).Replace(Path.DirectorySeparatorChar, '.')
					.Replace(Path.AltDirectorySeparatorChar, '.')
					.Replace(" ", "");
				if (!string.IsNullOrEmpty(ns))
				{
					localNamespace += ".";
					localNamespace += ns;
				}
			}
			else if (resxFolder.StartsWith(projectFolder, StringComparison.OrdinalIgnoreCase))
			{
				localNamespace += resxFolder.Substring(projectFolder.Length)
					.Replace(Path.DirectorySeparatorChar, '.')
					.Replace(Path.AltDirectorySeparatorChar, '.')
					.Replace(" ", "");
			}

			return localNamespace;
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static string GetClassNameFromPath(string resxFilePath)
	{
		//Fix issues with files that have names like xxx.aspx.resx
		var className = resxFilePath;
		while (className.Contains("."))
		{
			className = Path.GetFileNameWithoutExtension(className);
		}

		return className;
	}
}
