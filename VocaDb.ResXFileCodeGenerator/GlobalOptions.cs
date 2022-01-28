using Microsoft.CodeAnalysis.Diagnostics;

namespace VocaDb.ResXFileCodeGenerator;

internal record GlobalOptions
{
	private GlobalOptions(AnalyzerConfigOptions options)
	{
		ProjectFullPath = string.Empty;
		RootNamespace = string.Empty;
		if (!options.TryGetValue("build_property.MSBuildProjectFullPath", out var projectFullPath))
			return;
		ProjectFullPath = projectFullPath;
		if (!options.TryGetValue("build_property.RootNamespace", out var rootNamespace))
			return;
		RootNamespace = rootNamespace;
		// Code from: https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md#consume-msbuild-properties-and-metadata
		PublicClassGlobal = false;
		if (options.TryGetValue("build_property.ResXFileCodeGenerator_PublicClass", out var publicClassSwitch))
			PublicClassGlobal = publicClassSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);

		NullForgivingOperators =
			options.TryGetValue("build_property.ResXFileCodeGenerator_NullForgivingOperators", out var nullForgivingOperatorsSwitch) &&
			nullForgivingOperatorsSwitch is { Length: > 0 } &&
			nullForgivingOperatorsSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);
		
		StaticClass = !(options.TryGetValue("build_property.ResXFileCodeGenerator_StaticClass", out var staticClassSwitch) &&
		                staticClassSwitch is { Length: > 0 } &&
		                staticClassSwitch.Equals("false", StringComparison.OrdinalIgnoreCase));

		Valid = true;
	}

	public static GlobalOptions Select(AnalyzerConfigOptionsProvider provider, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		return new(provider.GlobalOptions);
	}

	public string RootNamespace { get; }

	public string ProjectFullPath { get; }

	public bool Valid { get; }

	public bool StaticClass { get; }

	public bool NullForgivingOperators { get; }

	public bool PublicClassGlobal { get; }
}
