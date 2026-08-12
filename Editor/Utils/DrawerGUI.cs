// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Reflection;
	using UnityObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;

	internal static class DrawerGUI
	{
		public static void DrawTex(in Texture2D tex, in Rect area, in Rect coords, Color color)
		{
			if (!tex)
			{
				return;
			}

			var tc = GUI.color;
			GUI.color = color;

			using (new GUI.ClipScope(area))
			{
				var sx = 1f / coords.size.x;
				var sy = 1f / coords.size.y;
				var ir = area;
				ir.size = new Vector2
				(
					sx * area.width,
					sy * area.height
				);
				ir.position = new Vector2
				(
					-coords.position.x * area.width * sx,
					-coords.position.y * area.height * sy
				);
				GUI.DrawTexture(ir, tex, ScaleMode.StretchToFill);
			}
			
			GUI.color = tc;
		}

		// returns value depending on what skin editor is using
		public static T PickSkin<T>(in T dark, in T light)
		{
			return EditorGUIUtility.isProSkin ? dark : light;
		}

		private static readonly GUIContent _dummyLabel = new();
		
		public static bool PopupButton(in Rect pos, in string label)
		{
			_dummyLabel.text = label;
			return PopupButton(pos, _dummyLabel);
		}
		
		public static bool PopupButton(in Rect pos, in GUIContent label)
		{
			return EditorGUI.DropdownButton(pos, label, FocusType.Keyboard);
		}
		public static void PrefixLabel(ref Rect pos, GUIContent l, FieldInfo fo)
		{
			if (l == GUIContent.none || fo.IsArray())
			{
				return;
			}
			pos = EditorGUI.PrefixLabel(pos, l);
		}

		public static void MutedInfo(in Rect pos, in string msg)
		{
			GUI.Box(pos, GUIContent.none);
			EditorGUI.LabelField(pos, msg, EditorStyles.centeredGreyMiniLabel);
		}
	}
}

#endif