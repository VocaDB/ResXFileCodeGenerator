using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VocaDb.ResXFileCodeGenerator;

internal static class AnalyzerConfigOptionsExtensions
{
	public static string? GetValueOrDefault(this AnalyzerConfigOptions options, string key) => options.TryGetValue(key, out var value) ? value : null;
}

[Generator]
public class SourceGenerator : ISourceGenerator
{
	private static readonly IGenerator s_generator = new StringBuilderGenerator();

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

	// Code from: https://github.com/dotnet/ResXResourceManager/blob/c8b5798d760f202a1842a74191e6010c6e8bbbc0/src/ResXManager.VSIX/Visuals/MoveToResourceViewModel.cs#L120
	private static string GetLocalNamespace(string? resxPath, string? projectPath, string? rootNamespace)
	{
		try
		{
			if (resxPath is null)
				return string.Empty;

			var resxFolder = Path.GetDirectoryName(resxPath);
			var projectFolder = Path.GetDirectoryName(projectPath);
			rootNamespace ??= string.Empty;

			if (resxFolder is null || projectFolder is null)
				return string.Empty;

			var localNamespace = rootNamespace;
			if (resxFolder.StartsWith(projectFolder, StringComparison.OrdinalIgnoreCase))
			{
				localNamespace += resxFolder.Substring(projectFolder.Length)
					.Replace(Path.DirectorySeparatorChar, '.')
					.Replace(Path.AltDirectorySeparatorChar, '.')
					.Replace("My Project", "My");
			}

			return localNamespace;
		}
		catch (Exception)
		{
			return string.Empty;
		}
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
			using var resxStream = File.OpenRead(resxFile.Path);
			var localNamespace = GetLocalNamespace(resxFile.Path, projectFullPath, rootNamespace);
			var customToolNamespace = context.AnalyzerConfigOptions.GetOptions(resxFile).GetValueOrDefault("build_metadata.EmbeddedResource.CustomToolNamespace").NullIfEmpty();
			var className = Path.GetFileNameWithoutExtension(resxFile.Path);
			var source = s_generator.Generate(
				resxStream: resxStream,
				options: new GeneratorOptions(
					LocalNamespace: localNamespace,
					CustomToolNamespace: customToolNamespace,
					ClassName: className,
					PublicClass: true
				)
			);
			context.AddSource($"{localNamespace}.{className}.g.cs", source);
		}
	}

	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
