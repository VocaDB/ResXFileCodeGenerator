using System.Globalization;
using System.Resources;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace VocaDb.ResXFileCodeGenerator;

public sealed partial class StringBuilderGenerator : IGenerator
{
	private void GenerateResourceManager(FileOptions options, SourceText content, string indent, string containerClassName,
		StringBuilder builder, List<Diagnostic> errorsAndWarnings, CancellationToken cancellationToken)
	{
					GenerateResourceManagerMembers(builder, indent, containerClassName, options);

		var members = ReadResxFile(content);
		if (members == null)
		{
			return;
		}

		HashSet<string> alreadyAddedMembers = new() { Constants.CultureInfoVariable };
		foreach (var (key, value, line) in members)
		{
			cancellationToken.ThrowIfCancellationRequested();
			CreateMember(indent, builder, options, key, value, line, alreadyAddedMembers, errorsAndWarnings,
				containerClassName);
		}
	}
	
	private static void CreateMember(string indent, StringBuilder builder, FileOptions options, string name, string value,
		IXmlLineInfo line, HashSet<string> alreadyAddedMembers, List<Diagnostic> errorsAndWarnings,
		string containerclassname)
	{
		if (!GenerateMember(indent, builder, options, name, value, line, alreadyAddedMembers, errorsAndWarnings, containerclassname, out var resourceAccessByName))
		{
			return;
		}

		if (resourceAccessByName)
		{
			builder.Append(" => ResourceManager.GetString(nameof(");
			builder.Append(name);
			builder.Append("), ");
		}
		else
		{
			builder.Append(@" => ResourceManager.GetString(""");
			builder.Append(name.Replace(@"""", @"\"""));
			builder.Append(@""", ");
		}

		builder.Append(Constants.CultureInfoVariable);
		builder.Append(")");
		builder.Append(options.NullForgivingOperators ? "!" : null);
		builder.AppendLine(";");
	}

	private static void AppendResourceManagerUsings(StringBuilder builder)
	{
		builder.Append("using ");
		builder.Append(Constants.SystemGlobalization);
		builder.AppendLine(";");

		builder.Append("using ");
		builder.Append(Constants.SystemResources);
		builder.AppendLine(";");

		builder.AppendLine();
	}

	private static void GenerateResourceManagerMembers(StringBuilder builder, string indent, string containerClassName, FileOptions options)
	{
		builder.Append(indent);
		builder.Append("private static ");
		builder.Append(nameof(ResourceManager));
		builder.Append("? ");
		builder.Append(Constants.s_resourceManagerVariable);
		builder.AppendLine(";");

		builder.Append(indent);
		builder.Append("public static ");
		builder.Append(nameof(ResourceManager));
		builder.Append(" ");
		builder.Append(Constants.ResourceManagerVariable);
		builder.Append(" => ");
		builder.Append(Constants.s_resourceManagerVariable);
		builder.Append(" ??= new ");
		builder.Append(nameof(ResourceManager));
		builder.Append("(\"");
		builder.Append(options.EmbeddedFilename);
		builder.Append("\", typeof(");
		builder.Append(containerClassName);
		builder.AppendLine(").Assembly);");

		builder.Append(indent);
		builder.Append("public ");
		builder.Append(options.StaticMembers ? "static " : "");
		builder.Append(nameof(CultureInfo));
		builder.Append("? ");
		builder.Append(Constants.CultureInfoVariable);
		builder.AppendLine(" { get; set; }");
	}
}
