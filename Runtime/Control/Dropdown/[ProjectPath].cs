// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;

	public enum EProjectPath
	{
		File,
		Folder
	}

	/// <summary>
	/// Select path to project file
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ProjectPathAttribute : PropertyAttribute
	{
		internal static readonly string[] DEF_IGNORE_FOLDERS =
		{
			"Library",
			"obj",
			"Temp",
			"Packages",
			".git",
			".svn",
		};

		internal EProjectPath mode { get; }
		internal string path { get; }
		internal string wildcard { get; }
		internal bool recursive { get; }
		internal string[] ignoreFolders { get; }

		public ProjectPathAttribute
		(
			EProjectPath mode = EProjectPath.File,
			string path = "Assets",
			string wildcard = "*",
			bool recursive = true,
			string[] ignoreFolders = null
		)
		{
			this.ignoreFolders = ignoreFolders ?? DEF_IGNORE_FOLDERS;
			this.mode = mode;
			this.recursive = recursive;
			this.path = path;
			this.wildcard = wildcard;
		}
	}

	
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(ProjectPathAttribute))]
	internal sealed class _ProjectPathAttribute : PropertyDrawer
	{

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			// label
			DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);

			// type != string
			if (!prop.IsString())
			{
				DrawerGUI.MutedInfo(pos, PluginConstants.Msg.FIELD_NON_STRING);
				return;
			}

			var attr = (ProjectPathAttribute)attribute;

			DrawPopup(pos, prop, attr);
		}
		
		public const string _EMPTY_LABEL = PluginConstants.Label.POPUP_UNSET;

		private static readonly Color _ICON_COLOR = EditorGUIUtility.isProSkin
		? Color.white * 0.8f
		: Color.black * 0.65f;

		private static void DrawPopup(Rect pos, SerializedProperty prop, ProjectPathAttribute a)
		{
			var l = !string.IsNullOrEmpty(prop.stringValue)
			? prop.stringValue
			: _EMPTY_LABEL;

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				l = l.Substring(l.LastIndexOf('/') + 1);
			}

			var icoRect = pos.SliceLeft(pos.height).Resized(-pos.height * 0.1f);

			var ico = a.mode == EProjectPath.File
			? EAtlasIcon.File
			: EAtlasIcon.Folder;
			
			PluginAtlas.DrawIcon(icoRect, ico,_ICON_COLOR);

			if (EditorGUI.DropdownButton(pos, new GUIContent(l, prop.stringValue), FocusType.Keyboard))
			{
				if (a.mode == EProjectPath.File)
				{
					GetFileMenu(a.path, a, prop).DropDown(pos);
				}
				else
				{
					GetFolderMenu(a.path, a, prop).DropDown(pos);
				}
			}
			
			
		}

		private static GenericMenu GetFileMenu(string prefix, ProjectPathAttribute a, SerializedProperty prop)
		{
			var menu = new GenericMenu();

			if (prefix.Length > 0)
			{
				menu.AddDisabledItem(new GUIContent(prefix.Replace("/", " | ")));
				menu.AddSeparator("");
			}

			menu.AddItem(new GUIContent(_EMPTY_LABEL), string.IsNullOrEmpty(prop.stringValue), () => {
				prop.stringValue = null;
				prop.serializedObject.ApplyModifiedProperties();
			});

			menu.AddSeparator("");

			var paths = ListFiles(a);
			var pflength = prefix.Length;

			foreach (var p in paths)
			{
				var l = pflength > 0 ? p.Substring(pflength + 1) : p;
				menu.AddItem(new GUIContent(l), prop.stringValue == p, () => {
					prop.stringValue = p;
					prop.serializedObject.ApplyModifiedProperties();
				});

			}
			return menu;
		}
		
		private static GenericMenu GetFolderMenu(string prefix, ProjectPathAttribute a, SerializedProperty prop)
		{
			var menu = new GenericMenu();

			if (prefix.Length > 0)
			{
				menu.AddDisabledItem(new GUIContent(prefix.Replace("/", "|")));
				menu.AddSeparator(string.Empty);
			}

			Action<string> setFn = v =>
			{
				prop.stringValue = v;
				prop.serializedObject.ApplyModifiedProperties();
			};

			menu.AddItem(new GUIContent(_EMPTY_LABEL), string.IsNullOrEmpty(prop.stringValue), () => setFn.Invoke(""));
			menu.AddSeparator(string.Empty);

			var paths = ListFolders(a);
			var pflength = prefix.Length;

			foreach (var p in paths)
			{
				var l = pflength > 0 ? p.Substring(pflength + 1) : p;
				menu.AddItem(new GUIContent(l), prop.stringValue == p, () => setFn.Invoke(p));

			}
			return menu;
		}

		private static IReadOnlyList<string> ListFolders(ProjectPathAttribute a)
		{
			return ListProjectPaths
			(
				EProjectPath.Folder,
				a.path,
				a.wildcard,
				a.recursive,
				a.ignoreFolders
			);
		}

		private static IReadOnlyList<string> ListFiles(ProjectPathAttribute a)
		{
			return ListProjectPaths
			(
				EProjectPath.File,
				a.path,
				a.wildcard,
				a.recursive,
				a.ignoreFolders
			);
		}
		
		private static bool Includes(string[] patterns, string path)
		{
			foreach (var p in patterns)
			{
				if (path.StartsWith(p))
				{
					return true;
				}
			}
			return false;
		}

		private static IReadOnlyList<string> ListProjectPaths
		(
			EProjectPath mode,
			string rootFolder,
			string searchPattern,
			bool recursive,
			string[] ignorePatterns
		)
		{
			ignorePatterns ??= Array.Empty<string>();
			var root = Application.dataPath.Slice(-7);
			var path = Path.Combine(root, rootFolder).Replace("\\", "/");
			if (!Directory.Exists(path))
			{
				return Array.Empty<string>();
			}

			var searchOption = recursive
			? SearchOption.AllDirectories
			: SearchOption.TopDirectoryOnly;
		
			var paths = mode == EProjectPath.File
			? Directory.GetFiles(path, searchPattern, searchOption)
			: Directory.GetDirectories(path, searchPattern, searchOption);
			
			List<string> outPaths = new();

			foreach (var p in paths)
			{
				var fPath = p.Substring(root.Length + 1).Replace("\\", "/");
				if (Includes(ignorePatterns, fPath))
				{
					continue;
				}
				outPaths.Add(fPath);
			}
			outPaths.Sort();
			return outPaths;
		}


	}
}

#endif