// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	internal static class Array_
	{
		public static string[] Stringify<T>(this T[] arr)
		{
			if (typeof(T) == typeof(string)) { return arr as string[]; }
			var labels = new string[arr.Length];
			for (var i = 0; i < arr.Length; i++) { labels[i] = arr[i].ToString(); }
			return labels;
		}
	}
}