using Microsoft.CodeAnalysis.Diagnostics;

namespace VocaDb.ResXFileCodeGenerator;

public readonly record struct FileOptions 
{
	public string InnerClassInstanceName { get; init; }
	public string InnerClassName { get; init; }
	public InnerClassVisibility InnerClassVisibility { get; init; }
	public bool PartialClass { get; init; }
	public bool StaticMembers { get; init; } = true;
	public GroupedAdditionalFile GroupedFile { get; init; }
	public bool StaticClass { get; init; }
	public bool NullForgivingOperators { get; init; }
	public bool PublicClass { get; init; }
	public string ClassName { get; init; }
	public string? CustomToolNamespace { get; init; }
	public string LocalNamespace { get; init; }
	public bool UseVocaDbResManager { get; init; }
	public string EmbeddedFilename { get; init; }
	public bool IsValid { get; init; }
	
	public FileOptions(
		GroupedAdditionalFile groupedFile,
		AnalyzerConfigOptions options,
		GlobalOptions globalOptions
	)
	{
		GroupedFile = groupedFile;
		var resxFilePath = groupedFile.MainFile.File.Path;

		var classNameFromFileName = Utilities.GetClassNameFromPath(resxFilePath);

		var detectedNamespace = Utilities.GetLocalNamespace(
			resxFilePath,
			options.TryGetValue("build_metadata.EmbeddedResource.Link", out var link) &&
			link is { Length: > 0 }
				? link
				: null,
			globalOptions.ProjectFullPath,
			globalOptions.ProjectName,
			globalOptions.RootNamespace);
		 
		EmbeddedFilename = string.IsNullOrEmpty(detectedNamespace) ? classNameFromFileName : $"{detectedNamespace}.{classNameFromFileName}";

		LocalNamespace =
			options.TryGetValue("build_metadata.EmbeddedResource.TargetPath", out var targetPath) &&
			targetPath is { Length: > 0 }
				? Utilities.GetLocalNamespace(
					resxFilePath, targetPath,
					globalOptions.ProjectFullPath,
					globalOptions.ProjectName,
					globalOptions.RootNamespace)
				: string.IsNullOrEmpty(detectedNamespace)
					? Utilities.SanitizeNamespace(globalOptions.ProjectName)
					: detectedNamespace;

		CustomToolNamespace =
			options.TryGetValue("build_metadata.EmbeddedResource.CustomToolNamespace", out var customToolNamespace) &&
			customToolNamespace is { Length: > 0 }
				? customToolNamespace
				: null;

		ClassName =
			options.TryGetValue("build_metadata.EmbeddedResource.ClassNamePostfix", out var perFileClassNameSwitch) &&
			perFileClassNameSwitch is { Length: > 0 }
				? classNameFromFileName + perFileClassNameSwitch
				: classNameFromFileName + globalOptions.ClassNamePostfix;

		NullForgivingOperators = globalOptions.NullForgivingOperators;

		PublicClass =
			options.TryGetValue("build_metadata.EmbeddedResource.PublicClass", out var perFilePublicClassSwitch) &&
			perFilePublicClassSwitch is { Length: > 0 }
				? perFilePublicClassSwitch.Equals("true", StringComparison.OrdinalIgnoreCase)
				: globalOptions.PublicClass;

		StaticClass =
			options.TryGetValue("build_metadata.EmbeddedResource.StaticClass", out var perFileStaticClassSwitch) &&
			perFileStaticClassSwitch is { Length: > 0 }
				? !perFileStaticClassSwitch.Equals("false", StringComparison.OrdinalIgnoreCase)
				: globalOptions.StaticClass;

		StaticMembers =
			options.TryGetValue("build_metadata.EmbeddedResource.StaticMembers", out var staticMembersSwitch) &&
			staticMembersSwitch is { Length: > 0 }
				? !staticMembersSwitch.Equals("false", StringComparison.OrdinalIgnoreCase)
				: globalOptions.StaticMembers;

		PartialClass =
			options.TryGetValue("build_metadata.EmbeddedResource.PartialClass", out var partialClassSwitch) &&
			partialClassSwitch is { Length: > 0 }
				? partialClassSwitch.Equals("true", StringComparison.OrdinalIgnoreCase)
				: globalOptions.PartialClass;

		InnerClassVisibility = globalOptions.InnerClassVisibility;
		if (
			options.TryGetValue("build_metadata.EmbeddedResource.InnerClassVisibility", out var innerClassVisibilitySwitch) &&
			Enum.TryParse(innerClassVisibilitySwitch, true, out InnerClassVisibility v) &&
			v != InnerClassVisibility.SameAsOuter
		)
		{
			InnerClassVisibility = v;
		}

		InnerClassName = globalOptions.InnerClassName;
		if (
			options.TryGetValue("build_metadata.EmbeddedResource.InnerClassName", out var innerClassNameSwitch) &&
			innerClassNameSwitch is { Length: > 0 }
		)
		{
			InnerClassName = innerClassNameSwitch;
		}

		InnerClassInstanceName = globalOptions.InnerClassInstanceName;
		if (
			options.TryGetValue("build_metadata.EmbeddedResource.InnerClassInstanceName", out var innerClassInstanceNameSwitch) &&
			innerClassInstanceNameSwitch is { Length: > 0 }
		)
		{
			InnerClassInstanceName = innerClassInstanceNameSwitch;
		}

		UseVocaDbResManager = globalOptions.UseVocaDbResManager;
		if (
			options.TryGetValue("build_metadata.EmbeddedResource.UseVocaDbResManager", out var genCodeSwitch) &&
			genCodeSwitch is { Length: > 0 }
		)
		{
			UseVocaDbResManager = genCodeSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);
		}

		IsValid = globalOptions.IsValid;
	}

	public static FileOptions Select(
		GroupedAdditionalFile file,
		AnalyzerConfigOptionsProvider options,
		GlobalOptions globalOptions
	)
	{
		return new(
			groupedFile: file,
			options: options.GetOptions(file.MainFile.File),
			globalOptions: globalOptions
		);
	}
}
