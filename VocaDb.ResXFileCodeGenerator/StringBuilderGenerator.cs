using System.Globalization;
using System.Resources;
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
	private static readonly Regex s_validMemberNamePattern =
		new(@"^[\p{L}\p{Nl}_][\p{Cf}\p{L}\p{Mc}\p{Mn}\p{Nd}\p{Nl}\p{Pc}]*$",
			RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly Regex s_invalidMemberNameSymbols = new(@"[^\p{Cf}\p{L}\p{Mc}\p{Mn}\p{Nd}\p{Nl}\p{Pc}]",
		RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static readonly DiagnosticDescriptor s_duplicateWarning = new("VocaDbResXFileCodeGenerator001",
		"Duplicate member",
		"Ignored added member '{0}'",
		"ResXFileCodeGenerator",
		DiagnosticSeverity.Warning,
		true);

	private static readonly DiagnosticDescriptor s_memberSameAsClassWarning = new("VocaDbResXFileCodeGenerator002",
		"Member same name as class",
		"Ignored member '{0}' has same name as class",
		"ResXFileCodeGenerator",
		DiagnosticSeverity.Warning,
		true);

	private static readonly DiagnosticDescriptor s_memberWithStaticError = new("VocaDbResXFileCodeGenerator003",
		"Incompatible settings",
		"Cannot have static members/class with an class instance",
		"ResXFileCodeGenerator",
		DiagnosticSeverity.Error,
		true);

	public (string generatedFileName, string SourceCode, IEnumerable<Diagnostic> ErrorsAndWarnings) Generate(
		FileOptions options, CancellationToken cancellationToken = default)
	{
		var errorsAndWarnings = new List<Diagnostic>();
		var generatedFileName = $"{options.LocalNamespace}.{options.ClassName}.g.cs";

		var content = options.File.GetText(cancellationToken);
		if (content is null) return (generatedFileName, "//ERROR reading file:" + options.File.Path, errorsAndWarnings);

		// HACK: netstandard2.0 doesn't support improved interpolated strings?
		var builder = GetBuilder(options.CustomToolNamespace ?? options.LocalNamespace);

		if (options.UseVocaDbResManager)
			AppendCodeUsings(builder);
		else
			AppendResourceManagerUsings(builder);

		builder.Append(options.PublicClass ? "public" : "internal");
		builder.Append(options.PartialClass ? " partial" : "");
		builder.Append(options.StaticClass ? " static class " : " class ");
		builder.AppendLine(options.ClassName);
		builder.AppendLine("{");

		var indent = "    ";
		string containerClassName = options.ClassName;

		if (options.InnerClassVisibility != InnerClassVisibility.NotGenerated)
		{
			containerClassName = string.IsNullOrEmpty(options.InnerClassName) ? "Resources" : options.InnerClassName;
			if (!string.IsNullOrEmpty(options.InnerClassInstanceName))
			{
				if (options.StaticClass || options.StaticMembers)
				{
					errorsAndWarnings.Add(Diagnostic.Create(s_memberWithStaticError,
						Location.Create(options.File.Path, new(), new())));
				}

				builder.Append(indent);
				builder.Append("public ");
				builder.Append(containerClassName);
				builder.Append(" ");
				builder.Append(options.InnerClassInstanceName);
				builder.AppendLine(" { get; } = new();");
				builder.AppendLine();
			}

			builder.Append(indent);
			builder.Append(options.InnerClassVisibility == InnerClassVisibility.SameAsOuter
				? options.PublicClass ? "public" : "internal"
				: options.InnerClassVisibility.ToString().ToLowerInvariant());
			builder.Append(options.PartialClass ? " partial" : "");
			builder.Append(options.StaticClass ? " static class " : " class ");

			builder.AppendLine(containerClassName);
			builder.Append(indent);
			builder.AppendLine("{");

			indent += "    ";

		}

		if (options.UseVocaDbResManager)
			GenerateCode(options, content, indent, containerClassName, builder, errorsAndWarnings, cancellationToken);
		else
			GenerateResourceManager(options, content, indent, containerClassName, builder, errorsAndWarnings, cancellationToken);

		if (options.InnerClassVisibility != InnerClassVisibility.NotGenerated)
		{
			builder.AppendLine("    }");
		}

		builder.AppendLine("}");

		return (generatedFileName, builder.ToString(), errorsAndWarnings);
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

	private static bool GenerateMember(string indent, StringBuilder builder, FileOptions options, string name, string neutralValue,
		IXmlLineInfo line, HashSet<string> alreadyAddedMembers, List<Diagnostic> errorsAndWarnings, string containerclassname,
		out bool resourceAccessByName)
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
			Location.Create(fileOptions.File.Path, new(),
				new(new(line.LineNumber - 1, line.LinePosition - 1),
					new(line.LineNumber - 1, line.LinePosition - 1 + memberName.Length)));

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

		builder.AppendLine();

		builder.Append(indent);
		builder.AppendLine("/// <summary>");

		builder.Append(indent);
		builder.Append("/// Looks up a localized string similar to ");
		builder.Append(HttpUtility.HtmlEncode(neutralValue.Trim().Replace("\r\n", "\n").Replace("\r", "\n")
			.Replace("\n", Environment.NewLine + indent + "/// ")));
		builder.AppendLine(".");

		builder.Append(indent);
		builder.AppendLine("/// </summary>");

		builder.Append(indent);
		builder.Append("public ");
		builder.Append(options.StaticMembers ? "static " : "");
		builder.Append("string");
		builder.Append(options.NullForgivingOperators ? null : "?");
		builder.Append(" ");
		builder.Append(memberName);
		return true;
	}

	private static StringBuilder GetBuilder(string withnamespace)
	{
		var builder = new StringBuilder();
		
		builder.AppendLine(Constants.AutoGeneratedHeader);
		builder.AppendLine("#nullable enable");

		builder.Append("namespace ");
		builder.Append(withnamespace);
		builder.AppendLine(";");

		return builder;
	}

}
