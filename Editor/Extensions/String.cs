// smidgens @ github

/*
 * String transforms
 */
namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Text.RegularExpressions;

	internal static partial class String_
	{
		public static string ToSentenceCase(this string str)
		{
			return Regex.Replace(str, "([A-Z]{1,2}|[0-9]+)", " $1").TrimStart();
		}
	}
}

/*
 * String transforms
 */
namespace Smidgenomics.Unity.Attributes.Editor
{
	internal static partial class String_
	{
		public static string Slice(this string s, in int v)
		{
			return v >= 0 ? s.Slice(v, 0) : s.Slice(0, v);
		}

		public static string Slice(this string s, in int start = 0, in int end = 0)
		{
			if (string.IsNullOrEmpty(s)) { return null; }
			if (start >= s.Length || start < 0) { return null; }
			if (end == 0) { return s.Substring(start); }
			var endIndex = end;
			if (endIndex < 0) { endIndex = s.Length + end; } // get index from end
			if (endIndex <= start) { return null; }
			return s.Substring(start, endIndex - start);
		}

		public static string ToFileName(this string path)
		{
			if (path.Length == 0) { return ""; }
			var ldot = path.LastIndexOf('.');
			var lslash = path.LastIndexOf('/') + 1;
			if(ldot < 0 || lslash == 0) { return path; }
			return path.Substring(lslash, ldot - lslash);
		}
	}
}

/*
 * Pattern matching
 */
namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Text.RegularExpressions;

	internal static partial class String_
	{
		public static bool MatchPattern(this string s, in string pattern)
		{
			return Regex.IsMatch(s, pattern.ToWildcardRegex());
		}

		public static string ToWildcardRegex(this string v)
		{
			return "^" + Regex.Escape(v).Replace("\\?", ".").Replace("\\*", ".*") + "$";
		}
	}


}