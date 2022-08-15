// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;

	public static partial class ProjectPath
	{
		/// <summary>
		/// Select path to project folder
		/// </summary>
		[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
		public class FolderPathAttribute : PropertyAttribute
		{
			public static readonly string[] DEFAULT_IGNORE_FOLDERS =
			{
				"Library",
				"obj",
				"Temp",
				".git",
				".svn",
			};

			public string Path { get; set; } = "";
			public string Pattern { get; set; } = "*";
			public bool Recursive { get; set; } = true;
			public string[] IgnoreFolders { get; set; } = DEFAULT_IGNORE_FOLDERS;

			public FolderPathAttribute(string rootPath, string wildcard = "*")
			{
				Path = rootPath;
				Pattern = wildcard;
			}
		}
	}

	
}