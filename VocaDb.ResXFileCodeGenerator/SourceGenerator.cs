using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	private static readonly IGenerator s_generator = new StringBuilderGenerator();

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var globalOptions = context.AnalyzerConfigOptionsProvider.Select(GlobalOptions.Select);

		var allResxFiles = context.AdditionalTextsProvider.Where(static af => af.Path.EndsWith(".resx"));

		var monitor = allResxFiles.Where(static af => Path.GetFileNameWithoutExtension(af.Path) == Utilities.GetBaseName(af.Path));

		//It is unclear from documentation at which stage one should stop the pipeline and do what in the RegisterSourceOutput
		//we stop after the USER cannot alter anything, assuming that AdditionalText has some fancy internal comparrison to make sure it is same if content is same
		var inputs = monitor
			.Combine(globalOptions)
			.Combine(context.AnalyzerConfigOptionsProvider)
			.Select(static (x, t) => FileOptions.Select(x.Left.Left, x.Right, x.Left.Right, t))
			.Where(static x => x.Valid);
			//.Select(static (file, token) => (file,
			//	content: file.File.GetText(token) is { } str ? new StringReader(str.ToString()) : null))
			//.Where(static x => x.content is not null)
			//.Select(static (fileAndContent, token) => s_generator.Generate(fileAndContent.content!, fileAndContent.file, token));

			context.RegisterSourceOutput(inputs, (ctx, file) =>
			{
				var content = file.File.GetText(ctx.CancellationToken);
				if (content is null) return;
				var reader = new StringReader(content.ToString());

				var (generatedFileName, sourceCode, errorsAndWarnings) = s_generator.Generate(reader, file, ctx.CancellationToken);
				foreach (var sourceErrorsAndWarning in errorsAndWarnings)
				{
					ctx.ReportDiagnostic(sourceErrorsAndWarning);
				}

				ctx.AddSource(generatedFileName, sourceCode);
			});
	}
}
