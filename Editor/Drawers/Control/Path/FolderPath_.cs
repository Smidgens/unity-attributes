// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(ProjectPath.FolderPathAttribute))]
	internal class FolderPath_ : PropertyDrawer
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

			DrawPopup(pos, prop, (ProjectPath.FolderPathAttribute)attribute);
		}

		private static void DrawPopup(Rect r, SP prop, ProjectPath.FolderPathAttribute a)
		{
			var l = !string.IsNullOrEmpty(prop.stringValue)
			? prop.stringValue.Replace("/", " / ")
			: EMPTY_LABEL;

			if (GUI.Button(r, l, EditorStyles.popup))
			{
				GetMenu(a.Path, a, prop).DropDown(r);
			}
		}

		private static GenericMenu GetMenu(string prefix, ProjectPath.FolderPathAttribute a, SP prop)
		{
			var menu = new GenericMenu();

			if (prefix.Length > 0)
			{
				menu.AddDisabledItem(new GUIContent(prefix.Replace("/", " | ")));
				menu.AddSeparator("");
			}

			Action<string> setFn = v =>
			{
				prop.stringValue = v;
				prop.serializedObject.ApplyModifiedProperties();
			};

			menu.AddItem(new GUIContent(EMPTY_LABEL), string.IsNullOrEmpty(prop.stringValue), () => setFn.Invoke(""));
			menu.AddSeparator("");

			var paths = ListFolders(a);
			var pflength = prefix.Length;

			foreach (var p in paths)
			{
				var l = pflength > 0 ? p.Substring(pflength + 1) : p;
				menu.AddItem(new GUIContent(l), prop.stringValue == p, () => setFn.Invoke(p));

			}
			return menu;
		}

		private static string[] ListFolders(ProjectPath.FolderPathAttribute a)
		{
			return IOUtility.ListProjectFolders
			(
				a.Path,
				a.Pattern,
				a.Recursive,
				a.IgnoreFolders
			);
		}
	}
}

#endif