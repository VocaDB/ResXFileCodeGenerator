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
		var monitor = allResxFiles.Collect().SelectMany((x,_) => GroupResxFiles.Group(x));

		// It is unclear from documentation at which stage one should stop the pipeline and do what in the RegisterSourceOutput
		// we stop after the USER cannot alter anything, assuming that AdditionalText has some fancy internal comparrison to make sure it is same if content is same
		var inputs = monitor
			.Combine(globalOptions)
			.Combine(context.AnalyzerConfigOptionsProvider)
			.Select(static (x, t) => FileOptions.Select(
				file: x.Left.Left,
				options: x.Right,
				globalOptions: x.Left.Right,
				token: t
			))
			.Where(static x => x.IsValid);
		//.Select(static (file, token) => (file,
		//	content: file.File.GetText(token) is { } str ? new StringReader(str.ToString()) : null))
		//.Where(static x => x.content is not null)
		//.Select(static (fileAndContent, token) => s_generator.Generate(fileAndContent.content!, fileAndContent.file, token));

		context.RegisterSourceOutput(inputs, (ctx, file) =>
		{
			var (generatedFileName, sourceCode, errorsAndWarnings) =
				s_generator.Generate(file, ctx.CancellationToken);
			foreach (var sourceErrorsAndWarning in errorsAndWarnings)
			{
				ctx.ReportDiagnostic(sourceErrorsAndWarning);
			}

			ctx.AddSource(generatedFileName, sourceCode);
		});

		var detectAllCombosOfResx = monitor.Collect().SelectMany((x, _) => GroupResxFiles.DetectChildComboes(x));
		context.RegisterSourceOutput(detectAllCombosOfResx, (ctx, combo) =>
		{
			var (generatedFileName, sourceCode, errorsAndWarnings) =
				s_generator.Generate(combo, ctx.CancellationToken);
			foreach (var sourceErrorsAndWarning in errorsAndWarnings)
			{
				ctx.ReportDiagnostic(sourceErrorsAndWarning);
			}

			ctx.AddSource(generatedFileName, sourceCode);
		});
	}
}
