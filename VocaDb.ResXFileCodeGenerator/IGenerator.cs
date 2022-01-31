using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public interface IGenerator
{
	string Generate(TextReader resxStream, FileOptions options, Action<Diagnostic>? reportError = null);
}
