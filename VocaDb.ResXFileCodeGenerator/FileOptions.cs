using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VocaDb.ResXFileCodeGenerator;

internal record FileOptions
{
	private FileOptions(AdditionalText file, AnalyzerConfigOptions options, GlobalOptions globalOptions)
	{
		File = file;
		var resxFilePath = file.Path;
		LocalNamespace = Utilities.GetLocalNamespace(
			resxFilePath,
			options.TryGetValue("build_metadata.EmbeddedResource.TargetPath", out var targetPath) &&
			targetPath is { Length: > 0 }
				? targetPath
				: null,
			globalOptions.ProjectFullPath,
			globalOptions.RootNamespace);
		CustomToolNamespace =
			options.TryGetValue("build_metadata.EmbeddedResource.CustomToolNamespace",
				out var customToolNamespace) && customToolNamespace is { Length: > 0 }
				? customToolNamespace
				: null;
		ClassName = Utilities.GetClassNameFromPath(resxFilePath);
		PublicClass =
			options.TryGetValue("build_metadata.EmbeddedResource.PublicClass", out var perFilePublicClassSwitch) &&
			perFilePublicClassSwitch is { Length: > 0 }
				? perFilePublicClassSwitch.Equals("true", StringComparison.OrdinalIgnoreCase)
				: globalOptions.PublicClassGlobal;
		NullForgivingOperators = globalOptions.NullForgivingOperators;
		StaticClass =
			options.TryGetValue("build_metadata.EmbeddedResource.StaticClass", out var perFileStaticClassSwitch) &&
			perFileStaticClassSwitch is { Length: > 0 }
				? !perFileStaticClassSwitch.Equals("false", StringComparison.OrdinalIgnoreCase)
				: globalOptions.StaticClass;
		FilePath= resxFilePath;

		Valid = globalOptions.Valid;
	}

	public AdditionalText File { get; }

	public string FilePath { get; }
	public bool StaticClass { get; }
	public bool NullForgivingOperators { get; }
	public bool PublicClass { get; }
	public string ClassName { get; }
	public string? CustomToolNamespace { get; }
	public string LocalNamespace { get; }
	public bool Valid { get; }

	public static FileOptions Select(AdditionalText file, AnalyzerConfigOptionsProvider options, GlobalOptions globalOptions, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		return new(file, options.GetOptions(file), globalOptions);
	}

}
