using Microsoft.CodeAnalysis;

namespace VocaDb.ResXFileCodeGenerator;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	private static readonly IGenerator s_generator = new StringBuilderGenerator();

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var globalOptions = context.AnalyzerConfigOptionsProvider.Select(GlobalOptions.Select)
			.WithComparer(EqualityComparer<GlobalOptions>.Default);

		var monitor = context.AdditionalTextsProvider
			.Where(af =>
				af.Path.EndsWith(".resx") &&
				Path.GetFileNameWithoutExtension(af.Path) == Utilities.GetBaseName(af.Path));

		var inputs = monitor
			.Combine(globalOptions)
			.Combine(context.AnalyzerConfigOptionsProvider)
			.Select((x, t) => FileOptions.Select(x.Left.Left, x.Right, x.Left.Right, t))
			.Where(x => x.Valid);

		context.RegisterSourceOutput(inputs, (ctx, file) =>
		{
			var filecontent = file.File.GetText(ctx.CancellationToken);
			if (filecontent is null)
			{
				return;
			}
			var source = s_generator.Generate(new StringReader(filecontent.ToString()), file, ctx.ReportDiagnostic);
			ctx.AddSource($"{file.LocalNamespace}.{file.ClassName}.g.cs", source);
		});
	}
}
