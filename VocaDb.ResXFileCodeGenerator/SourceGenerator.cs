using System.Globalization;
using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

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
			{
				return false;
			}

			if (languageName.StartsWith("qps-", StringComparison.Ordinal))
			{
				return true;
			}

			CultureInfo? culture = new CultureInfo(languageName);

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
	private static string GetBaseName(string filePath)
	{
		string? name = Path.GetFileNameWithoutExtension(filePath);
		string? innerExtension = Path.GetExtension(name);
		string? languageName = innerExtension.TrimStart('.');

		return IsValidLanguageName(languageName) ? Path.GetFileNameWithoutExtension(name) : name;
	}

	// Code from: https://github.com/dotnet/ResXResourceManager/blob/c8b5798d760f202a1842a74191e6010c6e8bbbc0/src/ResXManager.VSIX/Visuals/MoveToResourceViewModel.cs#L120
	private static string GetLocalNamespace(string? resxPath, string? projectPath, string? rootNamespace)
	{
		try
		{
			if (resxPath is null)
			{
				return string.Empty;
			}

			string? resxFolder = Path.GetDirectoryName(resxPath);
			string? projectFolder = Path.GetDirectoryName(projectPath);
			rootNamespace ??= string.Empty;

			if (resxFolder is null || projectFolder is null)
			{
				return string.Empty;
			}

			string? localNamespace = rootNamespace;

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
		if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.MSBuildProjectFullPath", out string? projectFullPath))
		{
			return;
		}

		if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? rootNamespace))
		{
			return;
		}

		// Code from: https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md#consume-msbuild-properties-and-metadata
		bool publicClassGlobal = false;
		if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.ResXFileCodeGenerator_PublicClass", out string? publicClassSwitch))
		{
			publicClassGlobal = publicClassSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);
		}

		bool nullForgivingOperators =
			context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.ResXFileCodeGenerator_NullForgivingOperators", out string? nullForgivingOperatorsSwitch) &&
			nullForgivingOperatorsSwitch is { Length: > 0 } &&
			nullForgivingOperatorsSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);

		IEnumerable<AdditionalText>? resxFiles = context.AdditionalFiles
			.Where(af => af.Path.EndsWith(".resx"))
			.Where(af => Path.GetFileNameWithoutExtension(af.Path) == GetBaseName(af.Path));

		foreach (AdditionalText? resxFile in resxFiles)
		{
			using FileStream? resxStream = File.OpenRead(resxFile.Path);

			GeneratorOptions? options = new GeneratorOptions(
				LocalNamespace:
					GetLocalNamespace(
						context.AnalyzerConfigOptions.GetOptions(resxFile).TryGetValue("build_metadata.EmbeddedResource.TargetPath", out string? targetPath) && targetPath is { Length: > 0 }
							? targetPath
							: resxFile.Path,
						projectFullPath,
						rootNamespace),
				CustomToolNamespace:
					context.AnalyzerConfigOptions.GetOptions(resxFile).TryGetValue("build_metadata.EmbeddedResource.CustomToolNamespace", out string? customToolNamespace) && customToolNamespace is { Length: > 0 }
						? customToolNamespace
						: null,
				ClassName: Path.GetFileNameWithoutExtension(resxFile.Path),
				PublicClass:
					context.AnalyzerConfigOptions.GetOptions(resxFile).TryGetValue("build_metadata.EmbeddedResource.PublicClass", out string? perFilePublicClassSwitch) && perFilePublicClassSwitch is { Length: > 0 }
						? perFilePublicClassSwitch.Equals("true", StringComparison.OrdinalIgnoreCase)
						: publicClassGlobal,
				NullForgivingOperators: nullForgivingOperators
			);

			string? source = s_generator.Generate(resxStream, options);

			context.AddSource($"{options.LocalNamespace}.{options.ClassName}.g.cs", source);
		}
	}

	public void Initialize(GeneratorInitializationContext context) { }
}
