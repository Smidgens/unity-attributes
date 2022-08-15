// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;

	public static partial class ProjectPath
	{
		/// <summary>
		/// Select path to project file
		/// </summary>
		[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
		public class FilePathAttribute : PropertyAttribute
		{
			public static readonly string[] IGNORE_FOLDERS =
			{
				"Library",
				"obj",
				"Temp",
				"Packages",
				".git",
				".svn",
			};

			public string Path { get; private set; } = "Assets";
			public string Pattern { get; private set; } = "*";
			public bool Recursive { get; set; } = true;
			public string[] IgnoredFolders { get; } = IGNORE_FOLDERS;

			public FilePathAttribute() { }

			public FilePathAttribute(string rootPath, string wildcard = "*")
			{
				Path = rootPath;
				Pattern = wildcard;
			}
		}
	}

	
}