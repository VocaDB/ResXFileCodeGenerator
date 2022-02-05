using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace VocaDb.ResXFileCodeGenerator.Tests;

internal class AdditionalTextStub : AdditionalText
{
	private readonly SourceText? _text;
	public override SourceText? GetText(CancellationToken cancellationToken = new()) => _text;
	public AdditionalTextStub(string path, string? text = null)
	{
		_text = text == null ? null :  SourceText.From(text);
		Path = path;
	}

	public override string Path { get; }
}


