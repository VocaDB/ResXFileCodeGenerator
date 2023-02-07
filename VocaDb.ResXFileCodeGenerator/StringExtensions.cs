using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace VocaDb.ResXFileCodeGenerator;

internal static class StringExtensions
{
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);

	public static string? NullIfEmpty(this string? value) => value.IsNullOrEmpty() ? null : value;

	public static string Clip(this string str, int startIdx, int stopIdx)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = startIdx; i < stopIdx; i++)
		{
			sb.Append(str[i]);
		}
		return sb.ToString();
	}
}
