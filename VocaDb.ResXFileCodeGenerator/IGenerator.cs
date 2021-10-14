namespace VocaDb.ResXFileCodeGenerator
{
	public sealed record GeneratorOptions(string LocalNamespace, string? CustomToolNamespace, string ClassName);

	public interface IGenerator
	{
		string Generate(Stream resxStream, GeneratorOptions options);
	}
}
