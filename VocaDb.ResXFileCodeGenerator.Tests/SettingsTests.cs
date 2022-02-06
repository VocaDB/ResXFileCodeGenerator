using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace VocaDb.ResXFileCodeGenerator.Tests;

public class SettingsTests
{
	private static readonly GlobalOptions s_globalOptions = GlobalOptions.Select(
		provider: new AnalyzerConfigOptionsProviderStub(
			globalOptions: new AnalyzerConfigOptionsStub
			{
				RootNamespace = "required1",
				MSBuildProjectFullPath = "required2"
			},
			fileOptions: null!
		),
		token: default
	);

	[Fact]
	public void GlobalDefaults()
	{
		var globalOptions = s_globalOptions;
		globalOptions.RootNamespace.Should().Be("required1");
		globalOptions.ProjectFullPath.Should().Be("required2");
		globalOptions.InnerClassName.Should().BeNullOrEmpty();
		globalOptions.ClassNamePostfix.Should().BeNullOrEmpty();
		globalOptions.InnerClassInstanceName.Should().BeNullOrEmpty();
		globalOptions.InnerClassVisibility.Should().Be(InnerClassVisibility.NotGenerated);
		globalOptions.NullForgivingOperators.Should().Be(false);
		globalOptions.StaticClass.Should().Be(true);
		globalOptions.StaticMembers.Should().Be(true);
		globalOptions.PublicClass.Should().Be(false);
		globalOptions.PartialClass.Should().Be(false);
		globalOptions.IsValid.Should().Be(true);
	}

	[Fact]
	public void GlobalSettings_CanReadAll()
	{
		var globalOptions = GlobalOptions.Select(
			provider: new AnalyzerConfigOptionsProviderStub(
				globalOptions: new AnalyzerConfigOptionsStub
				{
					RootNamespace = "required1", MSBuildProjectFullPath = "required2",
					ResXFileCodeGenerator_InnerClassName = "test1",
					ResXFileCodeGenerator_InnerClassInstanceName = "test2",
					ResXFileCodeGenerator_ClassNamePostfix= "test3",
					ResXFileCodeGenerator_InnerClassVisibility = "public",
					ResXFileCodeGenerator_NullForgivingOperators = "true",
					ResXFileCodeGenerator_StaticClass = "false",
					ResXFileCodeGenerator_StaticMembers = "false",
					ResXFileCodeGenerator_UseVocaDbResManager = "true",
					ResXFileCodeGenerator_PublicClass = "true",
					ResXFileCodeGenerator_PartialClass = "true",
				},
				fileOptions: null!
			),
			token: default
		);
		globalOptions.RootNamespace.Should().Be("required1");
		globalOptions.ProjectFullPath.Should().Be("required2");
		globalOptions.InnerClassName.Should().Be("test1");
		globalOptions.InnerClassInstanceName.Should().Be("test2");
		globalOptions.ClassNamePostfix.Should().Be("test3");
		globalOptions.InnerClassVisibility.Should().Be(InnerClassVisibility.Public);
		globalOptions.NullForgivingOperators.Should().Be(true);
		globalOptions.StaticClass.Should().Be(false);
		globalOptions.UseVocaDbResManager.Should().Be(true);
		globalOptions.StaticMembers.Should().Be(false);
		globalOptions.PublicClass.Should().Be(true);
		globalOptions.PartialClass.Should().Be(true);
		globalOptions.IsValid.Should().Be(true);
	}

	[Fact]
	public void FileDefaults()
	{
		var fileOptions = FileOptions.Select(
			file: new(
				mainFile: new AdditionalTextStub("Path1.resx"),
				subFiles: Array.Empty<AdditionalText>()
			),
			options: new AnalyzerConfigOptionsProviderStub(
				globalOptions: null!,
				fileOptions: new AnalyzerConfigOptionsStub()
			),
			globalOptions: s_globalOptions,
			token: default
		);
		fileOptions.InnerClassName.Should().BeNullOrEmpty();
		fileOptions.InnerClassInstanceName.Should().BeNullOrEmpty();
		fileOptions.InnerClassVisibility.Should().Be(InnerClassVisibility.NotGenerated);
		fileOptions.NullForgivingOperators.Should().Be(false);
		fileOptions.StaticClass.Should().Be(true);
		fileOptions.StaticMembers.Should().Be(true);
		fileOptions.PublicClass.Should().Be(false);
		fileOptions.PartialClass.Should().Be(false);
		fileOptions.UseVocaDbResManager.Should().Be(false);
		fileOptions.LocalNamespace.Should().Be("required1");
		fileOptions.CustomToolNamespace.Should().BeNullOrEmpty();
		fileOptions.File.Path.Should().Be("Path1.resx");
		fileOptions.ClassName.Should().Be("Path1");
		fileOptions.IsValid.Should().Be(true);
	}

	[Fact]
	public void File_PostFix()
	{
		var fileOptions = FileOptions.Select(
			file: new(
				mainFile: new AdditionalTextStub("Path1.resx"),
				subFiles: Array.Empty<AdditionalText>()
			),
			options: new AnalyzerConfigOptionsProviderStub(
				globalOptions: null!,
				fileOptions: new AnalyzerConfigOptionsStub { ClassNamePostfix = "test1" }
			),
			globalOptions: s_globalOptions,
			token: default
		);
		fileOptions.ClassName.Should().Be("Path1test1");
		fileOptions.IsValid.Should().Be(true);
	}

	[Fact]
	public void FileSettings_CanReadAll()
	{
		var fileOptions = FileOptions.Select(
			file: new(
				mainFile: new AdditionalTextStub("Path1.resx"),
				subFiles: Array.Empty<AdditionalText>()
			),
			options: new AnalyzerConfigOptionsProviderStub(
				globalOptions: null!,
				fileOptions: new AnalyzerConfigOptionsStub
				{
					RootNamespace = "required1", MSBuildProjectFullPath = "required2",
					CustomToolNamespace = "ns1",
					InnerClassName = "test1",
					InnerClassInstanceName = "test2",
					InnerClassVisibility = "public",
					NullForgivingOperators = "true",
					StaticClass = "false",
					StaticMembers = "false",
					PublicClass = "true",
					PartialClass = "true",
					UseVocaDbResManager = "true",
				}
			),
			globalOptions: s_globalOptions,
			token: default
		);
		fileOptions.InnerClassName.Should().Be("test1");
		fileOptions.InnerClassInstanceName.Should().Be("test2");
		fileOptions.InnerClassVisibility.Should().Be(InnerClassVisibility.Public);
		fileOptions.NullForgivingOperators.Should().Be(false);
		fileOptions.StaticClass.Should().Be(false);
		fileOptions.StaticMembers.Should().Be(false);
		fileOptions.PublicClass.Should().Be(true);
		fileOptions.PartialClass.Should().Be(true);
		fileOptions.IsValid.Should().Be(true);
		fileOptions.UseVocaDbResManager.Should().Be(true);
		fileOptions.LocalNamespace.Should().Be("required1");
		fileOptions.CustomToolNamespace.Should().Be("ns1");
		fileOptions.File.Path.Should().Be("Path1.resx");
		fileOptions.ClassName.Should().Be("Path1");
	}

	[Fact]
	public void FileSettings_RespectsGlobalDefaults()
	{
		var globalOptions = GlobalOptions.Select(
			provider: new AnalyzerConfigOptionsProviderStub(
				globalOptions: new AnalyzerConfigOptionsStub
				{
					RootNamespace = "required1",
					MSBuildProjectFullPath = "required2",
					ResXFileCodeGenerator_InnerClassName = "test1",
					ResXFileCodeGenerator_InnerClassInstanceName = "test2",
					ResXFileCodeGenerator_ClassNamePostfix= "test3",
					ResXFileCodeGenerator_InnerClassVisibility = "public",
					ResXFileCodeGenerator_NullForgivingOperators = "true",
					ResXFileCodeGenerator_StaticClass = "false",
					ResXFileCodeGenerator_StaticMembers = "false",
					ResXFileCodeGenerator_PublicClass = "true",
					ResXFileCodeGenerator_PartialClass = "true",
				},
				fileOptions: null!
			),
			token: default
		);
		var fileOptions = FileOptions.Select(
			file: new(
				mainFile: new AdditionalTextStub("Path1.resx"),
				subFiles: Array.Empty<AdditionalText>()
			),
			options: new AnalyzerConfigOptionsProviderStub(
				globalOptions: null!,
				fileOptions: new AnalyzerConfigOptionsStub()
			),
			globalOptions: globalOptions,
			token: default
		);
		fileOptions.InnerClassName.Should().Be("test1");
		fileOptions.InnerClassInstanceName.Should().Be("test2");
		fileOptions.InnerClassVisibility.Should().Be(InnerClassVisibility.Public);
		fileOptions.NullForgivingOperators.Should().Be(true);
		fileOptions.StaticClass.Should().Be(false);
		fileOptions.StaticMembers.Should().Be(false);
		fileOptions.PublicClass.Should().Be(true);
		fileOptions.PartialClass.Should().Be(true);
		fileOptions.IsValid.Should().Be(true);
		fileOptions.UseVocaDbResManager.Should().Be(false);
		fileOptions.LocalNamespace.Should().Be("required1");
		fileOptions.CustomToolNamespace.Should().BeNullOrEmpty();
		fileOptions.File.Path.Should().Be("Path1.resx");
		fileOptions.ClassName.Should().Be("Path1test3");
		fileOptions.IsValid.Should().Be(true);
	}

	private class AnalyzerConfigOptionsStub : AnalyzerConfigOptions
	{
		// ReSharper disable InconsistentNaming
		public string? MSBuildProjectFullPath { get; init; }
		public string? RootNamespace { get; init; }
		public string? ResXFileCodeGenerator_ClassNamePostfix { get; init; }
		public string? ResXFileCodeGenerator_PublicClass { get; init; }
		public string? ResXFileCodeGenerator_NullForgivingOperators { get; init; }
		public string? ResXFileCodeGenerator_StaticClass { get; init; }
		public string? ResXFileCodeGenerator_StaticMembers { get; init; }
		public string? ResXFileCodeGenerator_PartialClass { get; init; }
		public string? ResXFileCodeGenerator_InnerClassVisibility { get; init; }
		public string? ResXFileCodeGenerator_InnerClassName { get; init; }
		public string? ResXFileCodeGenerator_InnerClassInstanceName { get; init; }
		public string? ResXFileCodeGenerator_UseVocaDbResManager { get; init; }
		public string? CustomToolNamespace { get; init; }
		public string? TargetPath { get; init; }
		public string? ClassNamePostfix { get; init; }
		public string? PublicClass { get; init; }
		public string? NullForgivingOperators { get; init; }
		public string? StaticClass { get; init; }
		public string? StaticMembers { get; init; }
		public string? PartialClass { get; init; }
		public string? InnerClassVisibility { get; init; }
		public string? InnerClassName { get; init; }
		public string? InnerClassInstanceName { get; init; }
		public string? UseVocaDbResManager { get; init; }

		// ReSharper restore InconsistentNaming

		public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
		{
			string? GetVal() =>
				key switch
				{
					"build_property.MSBuildProjectFullPath" => MSBuildProjectFullPath,
					"build_property.RootNamespace" => RootNamespace,
					"build_property.ResXFileCodeGenerator_UseVocaDbResManager" => ResXFileCodeGenerator_UseVocaDbResManager,
					"build_property.ResXFileCodeGenerator_ClassNamePostfix" => ResXFileCodeGenerator_ClassNamePostfix,
					"build_property.ResXFileCodeGenerator_PublicClass" => ResXFileCodeGenerator_PublicClass,
					"build_property.ResXFileCodeGenerator_NullForgivingOperators" => ResXFileCodeGenerator_NullForgivingOperators,
					"build_property.ResXFileCodeGenerator_StaticClass" => ResXFileCodeGenerator_StaticClass,
					"build_property.ResXFileCodeGenerator_StaticMembers" => ResXFileCodeGenerator_StaticMembers,
					"build_property.ResXFileCodeGenerator_PartialClass" => ResXFileCodeGenerator_PartialClass,
					"build_property.ResXFileCodeGenerator_InnerClassVisibility" => ResXFileCodeGenerator_InnerClassVisibility,
					"build_property.ResXFileCodeGenerator_InnerClassName" => ResXFileCodeGenerator_InnerClassName,
					"build_property.ResXFileCodeGenerator_InnerClassInstanceName" => ResXFileCodeGenerator_InnerClassInstanceName,
					"build_metadata.EmbeddedResource.CustomToolNamespace" => CustomToolNamespace,
					"build_metadata.EmbeddedResource.TargetPath" => TargetPath,
					"build_metadata.EmbeddedResource.ClassNamePostfix" => ClassNamePostfix,
					"build_metadata.EmbeddedResource.PublicClass" => PublicClass,
					"build_metadata.EmbeddedResource.NullForgivingOperators" => NullForgivingOperators,
					"build_metadata.EmbeddedResource.StaticClass" => StaticClass,
					"build_metadata.EmbeddedResource.StaticMembers" => StaticMembers,
					"build_metadata.EmbeddedResource.PartialClass" => PartialClass,
					"build_metadata.EmbeddedResource.InnerClassVisibility" => InnerClassVisibility,
					"build_metadata.EmbeddedResource.InnerClassName" => InnerClassName,
					"build_metadata.EmbeddedResource.InnerClassInstanceName" => InnerClassInstanceName,
					"build_metadata.EmbeddedResource.UseVocaDbResManager" => UseVocaDbResManager,
					_ => null
				};

			value = GetVal();
			return value is not null;
		}
	}

	private class AnalyzerConfigOptionsProviderStub : AnalyzerConfigOptionsProvider
	{
		private readonly AnalyzerConfigOptions _fileOptions;

		public override AnalyzerConfigOptions GlobalOptions { get; }

		public AnalyzerConfigOptionsProviderStub(AnalyzerConfigOptions globalOptions, AnalyzerConfigOptions fileOptions)
		{
			_fileOptions = fileOptions;
			GlobalOptions = globalOptions;
		}

		public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => throw new NotImplementedException();

		public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _fileOptions;

	}
}
