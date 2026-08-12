// // smidgens @ github
//
// #if UNITY_EDITOR
//
// namespace Smidgenomics.Unity.Attributes.Editor
// {
// 	using System;
// 	using System.Collections.Generic;
// 	using UnityEngine;
// 	using System.IO;
//
// 	internal static class IOUtility
// 	{
// 		private static bool Includes(string[] patterns, string path)
// 		{
// 			foreach (var p in patterns)
// 			{
// 				if (path.StartsWith(p))
// 				{
// 					return true;
// 				}
// 			}
// 			return false;
// 		}
//
// 		public static IReadOnlyList<string> ListProjectPaths
// 		(
// 			EProjectPath mode,
// 			string rootFolder,
// 			string searchPattern,
// 			bool recursive,
// 			string[] ignorePatterns
// 		)
// 		{
// 			ignorePatterns ??= Array.Empty<string>();
// 			var root = Application.dataPath.Slice(-7);
// 			var path = Path.Combine(root, rootFolder).Replace("\\", "/");
// 			if (!Directory.Exists(path))
// 			{
// 				return Array.Empty<string>();
// 			}
//
// 			var searchOption = recursive
// 			? SearchOption.AllDirectories
// 			: SearchOption.TopDirectoryOnly;
// 			
// 			var paths = mode == EProjectPath.File
// 			? Directory.GetFiles(path, searchPattern, searchOption)
// 			: Directory.GetDirectories(path, searchPattern, searchOption);
// 			
// 			List<string> outPaths = new();
//
// 			foreach (var p in paths)
// 			{
// 				var fPath = p.Substring(root.Length + 1).Replace("\\", "/");
// 				if (Includes(ignorePatterns, fPath))
// 				{
// 					continue;
// 				}
// 				outPaths.Add(fPath);
// 			}
// 			outPaths.Sort();
// 			return outPaths;
// 		}
// 	}
// }
//
// #endif