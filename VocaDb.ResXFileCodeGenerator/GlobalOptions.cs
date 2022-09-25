using Microsoft.CodeAnalysis.Diagnostics;

namespace VocaDb.ResXFileCodeGenerator;

public sealed record GlobalOptions // this must be a record or implement IEquatable<T>
{
	public string InnerClassInstanceName { get; }
	public bool StaticMembers { get; }
	public string InnerClassName { get; }
	public InnerClassVisibility InnerClassVisibility { get; }
	public bool PartialClass { get; }
	public string? RootNamespace { get; }
	public string ProjectFullPath { get; }
	public string ProjectName { get; }
	public bool StaticClass { get; }
	public bool NullForgivingOperators { get; }
	public bool PublicClass { get; }
	public string ClassNamePostfix { get; }
	public bool UseVocaDbResManager { get; }
	public bool IsValid { get; }

	public GlobalOptions(AnalyzerConfigOptions options)
	{
		IsValid = true;

		if (!options.TryGetValue("build_property.MSBuildProjectFullPath", out var projectFullPath))
		{
			IsValid = false;
		}
		ProjectFullPath = projectFullPath!;

		if (options.TryGetValue("build_property.RootNamespace", out var rootNamespace))
		{
			RootNamespace = rootNamespace;
		}
		
		if (!options.TryGetValue("build_property.MSBuildProjectName", out var projectName))
		{
			IsValid = false;
		}
		ProjectName = projectName!;

		// Code from: https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md#consume-msbuild-properties-and-metadata
		PublicClass =
			options.TryGetValue("build_property.ResXFileCodeGenerator_PublicClass", out var publicClassSwitch) &&
			publicClassSwitch is { Length: > 0 } &&
			publicClassSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);

		NullForgivingOperators =
			options.TryGetValue("build_property.ResXFileCodeGenerator_NullForgivingOperators", out var nullForgivingOperatorsSwitch) &&
			nullForgivingOperatorsSwitch is { Length: > 0 } &&
			nullForgivingOperatorsSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);

		StaticClass =
			!(
				options.TryGetValue("build_property.ResXFileCodeGenerator_StaticClass", out var staticClassSwitch) &&
				staticClassSwitch is { Length: > 0 } &&
				staticClassSwitch.Equals("false", StringComparison.OrdinalIgnoreCase)
			);

		StaticMembers =
			!(
				options.TryGetValue("build_property.ResXFileCodeGenerator_StaticMembers", out var staticMembersSwitch) &&
				staticMembersSwitch is { Length: > 0 } &&
				staticMembersSwitch.Equals("false", StringComparison.OrdinalIgnoreCase)
			);

		PartialClass =
			options.TryGetValue("build_property.ResXFileCodeGenerator_PartialClass", out var partialClassSwitch) &&
			partialClassSwitch is { Length: > 0 } &&
			partialClassSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);

		ClassNamePostfix = string.Empty;
		if (options.TryGetValue("build_property.ResXFileCodeGenerator_ClassNamePostfix", out var classNamePostfixSwitch))
		{
			ClassNamePostfix = classNamePostfixSwitch;
		}

		InnerClassVisibility = InnerClassVisibility.NotGenerated;
		if (
			options.TryGetValue("build_property.ResXFileCodeGenerator_InnerClassVisibility", out var innerClassVisibilitySwitch) &&
			Enum.TryParse(innerClassVisibilitySwitch, true, out InnerClassVisibility v)
		)
		{
			InnerClassVisibility = v;
		}

		InnerClassName = string.Empty;
		if (options.TryGetValue("build_property.ResXFileCodeGenerator_InnerClassName", out var innerClassNameSwitch))
		{
			InnerClassName = innerClassNameSwitch;
		}

		InnerClassInstanceName = string.Empty;
		if (options.TryGetValue("build_property.ResXFileCodeGenerator_InnerClassInstanceName", out var innerClassInstanceNameSwitch))
		{
			InnerClassInstanceName = innerClassInstanceNameSwitch;
		}

		UseVocaDbResManager = false;
		if (
			options.TryGetValue("build_property.ResXFileCodeGenerator_UseVocaDbResManager", out var genCodeSwitch) &&
			genCodeSwitch is { Length: > 0 } &&
			genCodeSwitch.Equals("true", StringComparison.OrdinalIgnoreCase)
		)
		{
			UseVocaDbResManager = true;
		}
	}

	public static GlobalOptions Select(AnalyzerConfigOptionsProvider provider, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		return new GlobalOptions(provider.GlobalOptions);
	}
}
