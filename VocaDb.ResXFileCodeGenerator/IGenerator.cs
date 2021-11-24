namespace VocaDb.ResXFileCodeGenerator;

public sealed record GeneratorOptions(
	string LocalNamespace,
	string? CustomToolNamespace,
	string ClassName,
	bool PublicClass
);

public interface IGenerator
{
	string Generate(Stream resxStream, GeneratorOptions options);
}
