// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(ProjectPath.FilePathAttribute))]
	internal class FilePath_ : PropertyDrawer
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

			DrawPopup(pos, prop, (ProjectPath.FilePathAttribute)attribute);
		}

		private static void DrawPopup(Rect r, SP prop, ProjectPath.FilePathAttribute a)
		{
			var l = !string.IsNullOrEmpty(prop.stringValue)
			? prop.stringValue.Replace("/", " / ")
			: EMPTY_LABEL;
			if (GUI.Button(r, l, EditorStyles.popup))
			{
				GetMenu(a.Path, a, prop).DropDown(r);
			}
		}

		private static GenericMenu GetMenu(string prefix, ProjectPath.FilePathAttribute a, SP prop)
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

		private static string[] WalkFolder(ProjectPath.FilePathAttribute a)
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