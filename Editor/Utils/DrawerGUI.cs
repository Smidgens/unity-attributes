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

		public static bool PopupButton(in Rect pos, in string label)
		{
			return GUI.Button(pos, label, EditorStyles.popup);
		}

		public static void Slider(
			in Rect pos,
			SP prop,
			in float min,
			in float max,
			in float step = -1f,
			in int precision = -1)
		{
			var validType =
			prop.propertyType is SerializedPropertyType.Integer or SerializedPropertyType.Float;

			if (!validType)
			{
				MutedInfo(pos, "Field is not numeric");
				return;
			}

			using (var check = new EditorGUI.ChangeCheckScope())
			{
				var val = prop.IsFloat() ? prop.floatValue : prop.intValue;

				float valueNew = EditorGUI.Slider(pos, val, min, max);
				if (check.changed)
				{
					if (precision >= 1) { valueNew = valueNew.Round(precision); }
					if(step > 0f)
					{
						valueNew = ((int)(valueNew / step)) * step;
					}
					valueNew = Mathf.Clamp(valueNew, min, max);

					if (prop.IsFloat())
					{
						prop.floatValue = valueNew;
					}
					else
					{
						prop.intValue = (int)valueNew;
					}
				}
			}
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