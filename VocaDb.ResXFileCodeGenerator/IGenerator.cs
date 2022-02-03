using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public interface IGenerator
{
	(string generatedFileName, string SourceCode, IEnumerable<Diagnostic> ErrorsAndWarnings) Generate(
		TextReader resxStream, FileOptions options, CancellationToken cancellationToken = default);
}
