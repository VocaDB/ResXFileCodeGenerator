using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

public interface IGenerator
{
	string Generate(StringReader resxStream, FileOptions options, Action<Diagnostic>? reportError = null);
}
