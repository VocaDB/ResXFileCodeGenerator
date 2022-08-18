using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace VocaDb.ResXFileCodeGenerator.Tests;

public class UtilitiesTests
{
 

	[Theory]
	[InlineData("Valid", "Valid")]
	[InlineData("_Valid", "_Valid")]
	[InlineData("Valid123", "Valid123")]
	[InlineData("Valid_123", "Valid_123")]
	[InlineData("Valid.123", "Valid.123")]
	[InlineData("8Ns", "_8Ns")]
	[InlineData("Ns+InvalidChar", "Ns_InvalidChar")]
	[InlineData("Ns..Folder...Folder2", "Ns.Folder.Folder2")]
	[InlineData("Ns.Folder.", "Ns.Folder")]
	[InlineData(".Ns.Folder", "Ns.Folder")]
	[InlineData("Folder with space", "Folder_with_space")]
	[InlineData("folder with .. space", "folder_with_._space")]
	public void SanitizeNamespace(string input, string expected)
	{
		Utilities.SanitizeNamespace(input).Should().Be(expected);
	}

	[Theory]
	[InlineData("Valid", "Valid")]
	[InlineData(".Valid", ".Valid")]
	[InlineData("8Ns", "8Ns")]
	[InlineData("..Ns", ".Ns")]
	public void SanitizeNamespaceWithoutFirstCharRules(string input, string expected)
	{
		Utilities.SanitizeNamespace(input, false).Should().Be(expected);
	}

}
