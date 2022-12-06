// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;

	internal static class ColorGUI
	{
		public static void HexColor(in Rect pos, SP prop)
		{
			// valid type?
			if (!prop.IsString())
			{
				DrawerGUI.MutedInfo(pos, EConstants.Info.FIELD_NON_STRING);
				return;
			}
			using(var check = new EditorGUI.ChangeCheckScope())
			{
				var newColor = EditorGUI.ColorField(pos, HexToColor(prop.stringValue));
				if(check.changed)
				{
					prop.stringValue = newColor.ToHexString();
				}
			}
		}

		private static Color HexToColor(in string hex)
		{
			if (ColorUtility.TryParseHtmlString(hex, out var c))
			{
				return c;
			}
			return Color.clear;
		}
	}
}

#endif