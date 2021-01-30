using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VocaDb.ResXFileCodeGenerator
{
	internal static class AnalyzerConfigOptionsExtensions
	{
		public static string? GetValueOrDefault(this AnalyzerConfigOptions options, string key) => options.TryGetValue(key, out var value) ? value : null;
	}

	[Generator]
	public class SourceGenerator : ISourceGenerator
	{
		// Code from: https://github.com/dotnet/ResXResourceManager/blob/0ec11bae232151400a5a8ca7b9835ac063c516d0/src/ResXManager.Model/ResourceManager.cs#L267
		private static bool IsValidLanguageName(string? languageName)
		{
			try
			{
				if (languageName.IsNullOrEmpty())
					return false;

				if (languageName.StartsWith("qps-", StringComparison.Ordinal))
					return true;

				var culture = new CultureInfo(languageName);

				while (!culture.IsNeutralCulture)
					culture = culture.Parent;

				return culture.LCID != 4096;
			}
			catch
			{
				return false;
			}
		}

		// Code from: https://github.com/dotnet/ResXResourceManager/blob/0ec11bae232151400a5a8ca7b9835ac063c516d0/src/ResXManager.Model/ProjectFileExtensions.cs#L77
		private static string GetBaseName(string filePath)
		{
			var name = Path.GetFileNameWithoutExtension(filePath);
			var innerExtension = Path.GetExtension(name);
			var languageName = innerExtension.TrimStart('.');

			return IsValidLanguageName(languageName) ? Path.GetFileNameWithoutExtension(name) : name;
		}

		// Code from: https://stackoverflow.com/questions/51179331/is-it-possible-to-use-path-getrelativepath-net-core2-in-winforms-proj-targeti/51180239#51180239
		private string GetRelativePath(string relativeTo, string path)
		{
			var uri = new Uri(relativeTo);
			var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			return rel.Contains(Path.DirectorySeparatorChar.ToString()) ? rel : $".{Path.DirectorySeparatorChar}{rel}";
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.MSBuildProjectFullPath", out var projectFullPath))
				return;

			if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace))
				return;

			var resxFiles = context.AdditionalFiles
				.Where(af => af.Path.EndsWith(".resx"))
				.Where(af => Path.GetFileNameWithoutExtension(af.Path) == GetBaseName(af.Path));
			foreach (var resxFile in resxFiles)
			{
				static string ReplaceSpecialChars(string value) => value.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.').Trim().Replace(' ', '_');
				using var resxStream = File.OpenRead(resxFile.Path);
				var defaultNamespace = $"{rootNamespace}.{ReplaceSpecialChars(Path.GetDirectoryName(GetRelativePath(projectFullPath, resxFile.Path)) ?? string.Empty)}";
				var customToolNamespace = context.AnalyzerConfigOptions.GetOptions(resxFile).GetValueOrDefault("build_metadata.EmbeddedResource.CustomToolNamespace").NullIfEmpty();
				var className = ReplaceSpecialChars(Path.GetFileNameWithoutExtension(resxFile.Path));
				using var generator = new Generator(resxStream, new GeneratorOptions(defaultNamespace, customToolNamespace, className));
				var unit = generator.Generate();
				context.AddSource($"{defaultNamespace}.{className}.g.cs", unit.ToFullString());
			}
		}

		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
