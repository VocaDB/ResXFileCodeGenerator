using System.Text;

namespace VocaDb.ResXFileCodeGenerator;

internal static class StringBuilderExtensions
{
	public static void AppendLineLF(this StringBuilder builder)
	{
		builder.Append('\n');
	}

	public static void AppendLineLF(this StringBuilder builder, string value)
	{
		builder.Append(value);
		builder.AppendLineLF();
	}
}
