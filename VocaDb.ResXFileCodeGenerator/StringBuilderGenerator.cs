using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace VocaDb.ResXFileCodeGenerator;

public sealed partial class StringBuilderGenerator : IGenerator
{
	private static readonly Regex s_validMemberNamePattern = new(
		pattern: @"^[\p{L}\p{Nl}_][\p{Cf}\p{L}\p{Mc}\p{Mn}\p{Nd}\p{Nl}\p{Pc}]*$",
		options: RegexOptions.Compiled | RegexOptions.CultureInvariant
	);

	private static readonly Regex s_invalidMemberNameSymbols = new(
		pattern: @"[^\p{Cf}\p{L}\p{Mc}\p{Mn}\p{Nd}\p{Nl}\p{Pc}]",
		options: RegexOptions.Compiled | RegexOptions.CultureInvariant
	);

	private static readonly DiagnosticDescriptor s_duplicateWarning = new(
		id: "VocaDbResXFileCodeGenerator001",
		title: "Duplicate member",
		messageFormat: "Ignored added member '{0}'",
		category: "ResXFileCodeGenerator",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true
	);

	private static readonly DiagnosticDescriptor s_memberSameAsClassWarning = new(
		id: "VocaDbResXFileCodeGenerator002",
		title: "Member same name as class",
		messageFormat: "Ignored member '{0}' has same name as class",
		category: "ResXFileCodeGenerator",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true
	);

	private static readonly DiagnosticDescriptor s_memberWithStaticError = new(
		id: "VocaDbResXFileCodeGenerator003",
		title: "Incompatible settings",
		messageFormat: "Cannot have static members/class with an class instance",
		category: "ResXFileCodeGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public (
		string GeneratedFileName,
		string SourceCode,
		IEnumerable<Diagnostic> ErrorsAndWarnings
	) Generate(
		FileOptions options,
		CancellationToken cancellationToken = default
	)
	{
		var errorsAndWarnings = new List<Diagnostic>();
		var generatedFileName = $"{options.LocalNamespace}.{options.ClassName}.g.cs";

		var content = options.GroupedFile.MainFile.File.GetText(cancellationToken);
		if (content is null) return (generatedFileName, "//ERROR reading file:" + options.GroupedFile.MainFile.File.Path, errorsAndWarnings);

		// HACK: netstandard2.0 doesn't support improved interpolated strings?
		var builder = GetBuilder(options.CustomToolNamespace ?? options.LocalNamespace);

		if (options.UseVocaDbResManager)
			AppendCodeUsings(builder);
		else
			AppendResourceManagerUsings(builder);

		builder.Append(options.PublicClass ? "public" : "internal");
		builder.Append(options.StaticClass ? " static" : string.Empty);
		builder.Append(options.PartialClass ? " partial class " : " class ");
		builder.AppendLineLF(options.ClassName);
		builder.AppendLineLF("{");

		var indent = "    ";
		string containerClassName = options.ClassName;

		if (options.InnerClassVisibility != InnerClassVisibility.NotGenerated)
		{
			containerClassName = string.IsNullOrEmpty(options.InnerClassName) ? "Resources" : options.InnerClassName;
			if (!string.IsNullOrEmpty(options.InnerClassInstanceName))
			{
				if (options.StaticClass || options.StaticMembers)
				{
					errorsAndWarnings.Add(Diagnostic.Create(
						descriptor: s_memberWithStaticError,
						location: Location.Create(
							filePath: options.GroupedFile.MainFile.File.Path,
							textSpan: new TextSpan(),
							lineSpan: new LinePositionSpan()
						)
					));
				}

				builder.Append(indent);
				builder.Append("public ");
				builder.Append(containerClassName);
				builder.Append(" ");
				builder.Append(options.InnerClassInstanceName);
				builder.AppendLineLF(" { get; } = new();");
				builder.AppendLineLF();
			}

			builder.Append(indent);
			builder.Append(options.InnerClassVisibility == InnerClassVisibility.SameAsOuter
				? options.PublicClass ? "public" : "internal"
				: options.InnerClassVisibility.ToString().ToLowerInvariant());
			builder.Append(options.StaticClass ? " static" : string.Empty);
			builder.Append(options.PartialClass ? " partial class " : " class ");

			builder.AppendLineLF(containerClassName);
			builder.Append(indent);
			builder.AppendLineLF("{");

			indent += "    ";

		}

		if (options.UseVocaDbResManager)
			GenerateCode(options, content, indent, containerClassName, builder, errorsAndWarnings, cancellationToken);
		else
			GenerateResourceManager(options, content, indent, containerClassName, builder, errorsAndWarnings, cancellationToken);

		if (options.InnerClassVisibility != InnerClassVisibility.NotGenerated)
		{
			builder.AppendLineLF("    }");
		}

		builder.AppendLineLF("}");

		return (
			GeneratedFileName: generatedFileName,
			SourceCode: builder.ToString(),
			ErrorsAndWarnings: errorsAndWarnings
		);
	}

	private static IEnumerable<(string key, string value, IXmlLineInfo line)>? ReadResxFile(SourceText content)
	{
		using var reader = new StringReader(content.ToString());

		if (XDocument.Load(reader, LoadOptions.SetLineInfo).Root is { } element)
			return element
				.Descendants()
				.Where(static data => data.Name == "data")
				.Select(static data => (data.Attribute("name")!.Value, data.Descendants("value").First().Value,
					(IXmlLineInfo)data.Attribute("name")!));

		return null;
	}

	private static bool GenerateMember(
		string indent,
		StringBuilder builder,
		FileOptions options,
		string name,
		string neutralValue,
		IXmlLineInfo line,
		HashSet<string> alreadyAddedMembers,
		List<Diagnostic> errorsAndWarnings,
		string containerclassname,
		out bool resourceAccessByName
	)
	{
		string memberName;

		if (s_validMemberNamePattern.IsMatch(name))
		{
			memberName = name;
			resourceAccessByName = true;
		}
		else
		{
			memberName = s_invalidMemberNameSymbols.Replace(name, "_");
			resourceAccessByName = false;
		}

		static Location GetMemberLocation(FileOptions fileOptions, IXmlLineInfo line, string memberName) =>
			Location.Create(
				filePath: fileOptions.GroupedFile.MainFile.File.Path,
				textSpan: new TextSpan(),
				lineSpan: new LinePositionSpan(
					start: new LinePosition(line.LineNumber - 1, line.LinePosition - 1),
					end: new LinePosition(line.LineNumber - 1, line.LinePosition - 1 + memberName.Length)
				)
			);

		if (!alreadyAddedMembers.Add(memberName))
		{
			errorsAndWarnings.Add(Diagnostic.Create(s_duplicateWarning, GetMemberLocation(options, line, memberName),
				memberName));
			return false;
		}

		if (memberName == containerclassname)
		{
			errorsAndWarnings.Add(Diagnostic.Create(s_memberSameAsClassWarning,
				GetMemberLocation(options, line, memberName), memberName));
			return false;
		}

		builder.AppendLineLF();

		builder.Append(indent);
		builder.AppendLineLF("/// <summary>");

		builder.Append(indent);
		builder.Append("/// Looks up a localized string similar to ");
		builder.Append(HttpUtility.HtmlEncode(neutralValue.Trim().Replace("\r\n", "\n").Replace("\r", "\n")
			.Replace("\n", Environment.NewLine + indent + "/// ")));
		builder.AppendLineLF(".");

		builder.Append(indent);
		builder.AppendLineLF("/// </summary>");

		builder.Append(indent);
		builder.Append("public ");
		builder.Append(options.StaticMembers ? "static " : string.Empty);
		builder.Append("string");
		builder.Append(options.NullForgivingOperators ? null : "?");
		builder.Append(" ");
		builder.Append(memberName);
		return true;
	}

	private static StringBuilder GetBuilder(string withnamespace)
	{
		var builder = new StringBuilder();
		
		builder.AppendLineLF(Constants.AutoGeneratedHeader);
		builder.AppendLineLF("#nullable enable");

		builder.Append("namespace ");
		builder.Append(withnamespace);
		builder.AppendLineLF(";");

		return builder;
	}

}
