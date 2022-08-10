using System.Globalization;
using System.Text.RegularExpressions;

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

			var dash = languageName.IndexOf('-');
			if (dash >= 4 || (dash == -1 && languageName.Length >= 4)) return false;

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

			var localNamespace = string.Empty;

			if (!string.IsNullOrWhiteSpace(targetPath))
			{
				localNamespace = Path.GetDirectoryName(targetPath)
					.Trim(Path.DirectorySeparatorChar)
					.Trim(Path.AltDirectorySeparatorChar)
					.Replace(Path.DirectorySeparatorChar, '.')
					.Replace(Path.AltDirectorySeparatorChar, '.'); 
			}
			else if (resxFolder.StartsWith(projectFolder, StringComparison.OrdinalIgnoreCase))
			{
				localNamespace = resxFolder
					.Substring(projectFolder.Length)
					.Trim(Path.DirectorySeparatorChar)
					.Trim(Path.AltDirectorySeparatorChar)
					.Replace(Path.DirectorySeparatorChar, '.')
					.Replace(Path.AltDirectorySeparatorChar, '.');
			}

			return (string.IsNullOrEmpty(rootNamespace)
				? SanitizeNamespace(localNamespace)
				// If we have a root namespace, namespace first char rules do not apply
				: $"{rootNamespace}.{SanitizeNamespace(localNamespace, sanitizeFirstChar: false)}")
				// It's possible we do not have either a root namespace or a local namespace
				.Trim('.'); 
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static string GetClassNameFromPath(string resxFilePath)
	{
		// Fix issues with files that have names like xxx.aspx.resx
		var className = resxFilePath;
		while (className.Contains("."))
		{
			className = Path.GetFileNameWithoutExtension(className);
		}

		return className;
	}

	public static string SanitizeNamespace(string ns, bool sanitizeFirstChar = true)
	{
		if(string.IsNullOrEmpty(ns))
			return ns;

		// A namespace must contain only alphabetic characters, decimal digits, dots and underscores, and must begin with an alphabetic character or underscore (_)
		// In case there are invalid chars we'll use same logic as Visual Studio and replace them with underscore (_) and append underscore (_) if project does not start with alphabetic or underscore (_)

		var sanitizedNs = Regex
			.Replace(ns, @"[^a-zA-Z0-9_\.]", "_");

		// Handle folder containing multiple dots, e.g. 'test..test2' or starting, ending with dots
		sanitizedNs = Regex
			.Replace(sanitizedNs, @"\.+", ".");

		if (sanitizeFirstChar)
			sanitizedNs = sanitizedNs.Trim('.');

		return sanitizeFirstChar
			// Handle namespace starting with digit
			? char.IsDigit(sanitizedNs[0]) ? $"_{sanitizedNs}" : sanitizedNs
			: sanitizedNs;
	}
}
