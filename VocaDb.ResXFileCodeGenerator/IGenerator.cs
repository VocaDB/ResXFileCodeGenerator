using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public interface IGenerator
{
	/// <summary>
	/// Generate source file with properties for each translated resource
	/// </summary>
	(string GeneratedFileName, string SourceCode, IEnumerable<Diagnostic> ErrorsAndWarnings)
		Generate(FileOptions options, CancellationToken cancellationToken = default);

	/// <summary>
	/// Generate helper functions to determine which translated resource to use in the current moment
	/// </summary>
	(string GeneratedFileName, string SourceCode, IEnumerable<Diagnostic> ErrorsAndWarnings)
		Generate(CultureInfoCombo combo, CancellationToken cancellationToken);
}
