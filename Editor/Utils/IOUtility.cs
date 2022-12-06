// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using System.IO;
	using System.Linq;

	internal static class IOUtility
	{
		public static string[] ListProjectFolders
		(
			string subPath,
			string matchPattern,
			bool recursive,
			params string[] ignorePatterns
		)
		{
			var so = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var root = Application.dataPath.Slice(-7);
			var path = Path.Combine(root, subPath).Replace("\\", "/");
			if (!Directory.Exists(path)) { return new string[0]; }
			var paths = Directory.GetDirectories(path, "*", so);
			var rl = root.Length;
			var pl = subPath.Length;
			return paths
			.Where(x => x.Substring(path.Length + 1).MatchPattern(matchPattern))
			.Select(x => {
				return x.Substring(root.Length + 1).Replace("\\", "/");
			})
			.Where(x => {
				return ignorePatterns.FirstOrDefault(y => x.StartsWith(y)) == null;
			})
			.OrderBy(x => x)
			.ToArray();
		}

		public static string[] ListProjectFiles
		(
			string subPath,
			string matchPattern,
			bool recursive,
			params string[] ignorePatterns
		)
		{
			//var sub = a.Path;
			//var wildcard = a.Pattern;
			//var so = a.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			//var ignored = a.IgnoredFolders != null ? a.IgnoredFolders : new string[0];

			var sub = subPath;
			var wildcard = matchPattern;
			var so = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var ignored = ignorePatterns ?? new string[0];

			var root = Application.dataPath.Slice(-7);
			var path = Path.Combine(root, sub).Replace("\\", "/");
			if (!Directory.Exists(path)) { return new string[0]; }
			var paths = Directory.GetFiles(path, "*", so);
			var rl = root.Length;
			var pl = sub.Length;
			return paths
			.Where(x => x.Substring(path.Length + 1).MatchPattern(wildcard))
			.Select(x => {
				return x.Substring(root.Length + 1).Replace("\\", "/");
			})
			.Where(x => {
				return ignored.FirstOrDefault(y => x.StartsWith(y)) == null;
			})
			.OrderBy(x => x)
			.ToArray();
		}
	}
}

#endif