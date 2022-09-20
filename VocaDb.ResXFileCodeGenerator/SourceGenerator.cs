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
		// Note: Each Resx file will get a hash (random guid) so we can easily differentiate in the pipeline when the file changed or just some options
		var monitor = allResxFiles.Collect().SelectMany((x, token) => GroupResxFiles.Group(x.Select(f => new AdditionalTextWithHash(f, Guid.NewGuid())).ToList(), token));
		
		var inputs = monitor
			.Combine(globalOptions)
			.Combine(context.AnalyzerConfigOptionsProvider)
			.Select(static (x, _) => FileOptions.Select(
				file: x.Left.Left,
				options: x.Right,
				globalOptions: x.Left.Right
			))
			.Where(static x => x.IsValid);

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

		var detectAllCombosOfResx = monitor.Collect().SelectMany((x, _) => GroupResxFiles.DetectChildCombos(x));
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
