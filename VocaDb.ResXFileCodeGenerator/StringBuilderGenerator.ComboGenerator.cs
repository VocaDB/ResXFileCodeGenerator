using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace VocaDb.ResXFileCodeGenerator
{

	public sealed partial class StringBuilderGenerator : IGenerator
	{
		static readonly Dictionary<int, List<int>> s_allChildren = new();

		/// <summary>
		/// Build all cultureinfo children
		/// </summary>
		static StringBuilderGenerator()
		{
			var all = CultureInfo.GetCultures(CultureTypes.AllCultures);

			foreach (var cultureInfo in all)
			{
				if (cultureInfo.LCID == 4096 || cultureInfo.IsNeutralCulture || cultureInfo.Name.IsNullOrEmpty())
				{
					continue;
				}
				var parent = cultureInfo.Parent;
				if (!s_allChildren.TryGetValue(parent.LCID, out var v))
					s_allChildren[parent.LCID] = v = new();
				v.Add(cultureInfo.LCID);
			}
		}

		public (
			string GeneratedFileName,
			string SourceCode,
			IEnumerable<Diagnostic> ErrorsAndWarnings
		) Generate(
			CultureInfoCombo combo,
			CancellationToken cancellationToken
		)
		{
			var definedLanguages = combo.GetDefinedLanguages();
			var builder = GetBuilder("VocaDb.ResXFileCodeGenerator");

			builder.AppendLine("internal static partial class Helpers");
			builder.AppendLine("{");

			builder.Append("    public static string GetString_");
			var functionNamePostFix = FunctionNamePostFix(definedLanguages);
			builder.Append(functionNamePostFix);
			builder.Append("(string fallback");
			foreach (var (name, _, _) in definedLanguages)
			{
				builder.Append(", ");
				builder.Append("string ");
				builder.Append(name);
			}

			builder.Append(") => ");
			builder.Append(Constants.SystemGlobalization);
			builder.AppendLine(".CultureInfo.CurrentUICulture.LCID switch");
			builder.AppendLine("    {");
			HashSet<int> already = new();
			foreach (var (name, lcid, _) in definedLanguages)
			{
				static IEnumerable<int> FindParents(int toFind)
				{
					yield return toFind;
					if (!s_allChildren.TryGetValue(toFind, out var v))
					{
						yield break;
					}

					foreach (var parents in v)
					{
						yield return parents;
					}
				}

				var findParents = FindParents(lcid).Except(already).ToList();
				foreach (var parent in findParents)
				{
					already.Add(parent);
					builder.Append("        ");
					builder.Append(parent);
					builder.Append(" => ");
					builder.Append(name.Replace('-', '_'));
					builder.AppendLine(",");
				}
			}

			builder.AppendLine("        _ => fallback");
			builder.AppendLine("    };");
			builder.AppendLine("}");

			return (
				GeneratedFileName: "VocaDb.ResXFileCodeGenerator." + functionNamePostFix + ".g.cs",
				SourceCode: builder.ToString(),
				ErrorsAndWarnings: Array.Empty<Diagnostic>()
			);
		}

		private static string FunctionNamePostFix(
			IReadOnlyList<(string Name, int LCID, AdditionalText File)> definedLanguages
		) => string.Join("_", definedLanguages.Select(x => x.LCID));

		private static void AppendCodeUsings(StringBuilder builder)
		{
			builder.AppendLine("using static VocaDb.ResXFileCodeGenerator.Helpers;");
			builder.AppendLine();
		}

		private void GenerateCode(
			FileOptions options,
			SourceText content,
			string indent,
			string containerClassName,
			StringBuilder builder,
			List<Diagnostic> errorsAndWarnings,
			CancellationToken cancellationToken
		)
		{
			var combo = new CultureInfoCombo(options.SubFiles);
			var definedLanguages = combo.GetDefinedLanguages();

			var fallback = ReadResxFile(content);
			var subfiles = definedLanguages.Select(lang =>
			{
				var subcontent = lang.File.GetText(cancellationToken);
				return subcontent is null
					? null
					: ReadResxFile(subcontent)?
						.GroupBy(x => x.key)
						.ToImmutableDictionary(x => x.Key, x => x.First().value);
			}).ToList();
			if (fallback is null || subfiles.Any(x => x is null))
			{
				builder.AppendFormat("//could not read {0} or one of its children", options.File.Path);
				return;
			}

			HashSet<string> alreadyAddedMembers = new();
			foreach (var (key, value, line) in fallback)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (
					!GenerateMember(
						indent,
						builder,
						options,
						key,
						value,
						line,
						alreadyAddedMembers,
						errorsAndWarnings,
						containerClassName,
						out _
					)
				)
				{
					continue;
				}

				builder.Append(" => GetString_");
				builder.Append(FunctionNamePostFix(definedLanguages));
				builder.Append("(");
				builder.Append(SymbolDisplay.FormatLiteral(value, true));

				foreach (var xml in subfiles)
				{
					builder.Append(", ");
					if (!xml.TryGetValue(key, out var langValue))
						langValue = value;
					builder.Append(SymbolDisplay.FormatLiteral(langValue, true));
				}

				builder.AppendLine(");");
			}
		}
	}
}

namespace Resources{
	using static VocaDb.ResXFileCodeGenerator.Helpers;
public static class ActivityEntrySortRuleNames
{

	/// <summary>
	/// Looks up a localized string similar to Oldest.
	/// </summary>
	public static string? CreateDate => GetString_1030_6("Oldest", "OldestDaDK", "OldestDa");

	/// <summary>
	/// Looks up a localized string similar to Newest.
	/// </summary>
	public static string? CreateDateDescending => GetString_1030_6("Newest", "NewestDaDK", "NewestDa");
}
}

namespace VocaDb.ResXFileCodeGenerator
{
	internal static class Helpers
	{
		public static string GetString_1030_6(params string[] p)
		{
			throw new NotImplementedException();
		}

	}
}
