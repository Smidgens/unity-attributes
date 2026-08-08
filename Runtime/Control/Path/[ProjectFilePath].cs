// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;

	/// <summary>
	/// Select path to project file
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ProjectFilePathAttribute : PropertyAttribute
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

		public ProjectFilePathAttribute() { }

		public ProjectFilePathAttribute(string rootPath, string wildcard = "*")
		{
			Path = rootPath;
			Pattern = wildcard;
		}
	}

	
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(ProjectFilePathAttribute))]
	internal sealed class _ProjectFilePathAttribute : PropertyDrawer
	{
		public const string EMPTY_LABEL = EConstants.Label.POPUP_DEFAULT;

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			// label
			DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);

			// type != string
			if (!prop.IsString())
			{
				DrawerGUI.MutedInfo(pos, EConstants.Info.FIELD_NON_STRING);
			}

			DrawPopup(pos, prop, (ProjectFilePathAttribute)attribute);
		}

		private static void DrawPopup(Rect r, SP prop, ProjectFilePathAttribute a)
		{
			var l = !string.IsNullOrEmpty(prop.stringValue)
				? prop.stringValue.Replace("/", " / ")
				: EMPTY_LABEL;
			if (GUI.Button(r, l, EditorStyles.popup))
			{
				GetMenu(a.Path, a, prop).DropDown(r);
			}
		}

		private static GenericMenu GetMenu(string prefix, ProjectFilePathAttribute a, SP prop)
		{
			var menu = new GenericMenu();

			if (prefix.Length > 0)
			{
				menu.AddDisabledItem(new GUIContent(prefix.Replace("/", " | ")));
				menu.AddSeparator("");
			}

			menu.AddItem(new GUIContent(EMPTY_LABEL), string.IsNullOrEmpty(prop.stringValue), () => {
				prop.stringValue = null;
				prop.serializedObject.ApplyModifiedProperties();
			});

			menu.AddSeparator("");

			var paths = WalkFolder(a);
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

		private static string[] WalkFolder(ProjectFilePathAttribute a)
		{
			return IOUtility.ListProjectFiles
			(
				a.Path,
				a.Pattern,
				a.Recursive,
				a.IgnoredFolders
			);
		}


	}
}

#endif