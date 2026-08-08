// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;
	using System;
	using System.Linq;
	using System.Reflection;
	using UnityObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;

	internal static class DrawerGUI
	{
		public const float PAD_FULL = 2f;
		public const float PAD_MINI = 5f;
		
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

		public static void ColorPreview(in Rect pos, in Color c)
		{
			EditorGUI.DrawRect(pos, c);
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

		public static void IntegerDropdown(in Rect pos, SP prop, in int[] options)
		{
			// valid type?
			if (prop.propertyType != SerializedPropertyType.Integer)
			{
				MutedInfo(pos, EConstants.Info.FIELD_NON_INT);
				return;
			}

			if (GUI.Button(pos, prop.intValue.ToString(), EditorStyles.popup))
			{
				MenuFactory
				.GetMenu(prop, options)
				.DropDown(pos);
			}
		}
		
		public static void Layer(in Rect pos, SP prop)
		{
			// invalid type
			if (prop.propertyType != SerializedPropertyType.Integer)
			{
				MutedInfo(pos, EConstants.Info.FIELD_NON_INT);
				return;
			}

			var currentValue = prop.intValue;

			string currentName = LayerMask.LayerToName(currentValue);

			var btnLabel = !string.IsNullOrEmpty(currentName)
			? $"{currentValue}: {currentName}"
			: "<none>";

			if (GUI.Button(pos, btnLabel, EditorStyles.popup))
			{
				var m = new GenericMenu();
				foreach (var layerIndex in Enumerable.Range(0, 31))
				{
					var name = LayerMask.LayerToName(layerIndex);
					if (string.IsNullOrEmpty(name))
					{
						continue;
					}

					var v = layerIndex;

					m.AddItem(new GUIContent($"{layerIndex}: {name}"), layerIndex == currentValue, () =>
					{
							prop.intValue = v;
							prop.serializedObject.ApplyModifiedProperties();
					});

				}
				m.DropDown(pos);
			}
		}

		public static void MutedInfo(in Rect pos, in string msg)
		{
			GUI.Box(pos, GUIContent.none);
			EditorGUI.LabelField(pos, msg, EditorStyles.centeredGreyMiniLabel);
		}

		public static void AssetThumbnail(in Rect pos, UnityObject o, in bool full = false)
		{
			GUI.Box(pos, GUIContent.none, GUI.skin.box);
			if (!o)
			{
				return;
			}

			if (full)
			{
				GUI.DrawTexture(pos.Resize(-PAD_FULL), AssetPreview.GetAssetPreview(o));
			}
			else
			{
				GUI.DrawTexture(pos.Resize(-PAD_MINI), AssetPreview.GetMiniThumbnail(o));
			}

			if (GUI.Button(pos, GUIContent.none, GUIStyle.none))
			{
				EditorGUIUtility.PingObject(o);
			}
		}

		public static bool TabButton(in Rect pos, bool value, in string label)
		{
			// background
			EditorGUI.DrawRect(pos, _TOGGLE_COLORS[value.ToInt()]);
			if (GUI.Button(pos, "", _ToggleTabStyle.Value))
			{
				value = !value;
			}
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			EditorGUI.LabelField(pos, label, _ToggleTabStyle.Value);
			return value;
		}

		private static readonly Color[] _TOGGLE_COLORS =
		{
			Color.black * 0.1f, // false
			Color.white * 0.5f, // true
		};

		private static readonly Lazy<GUIStyle> _ToggleTabStyle = new Lazy<GUIStyle>(() =>
		{
			var s = new GUIStyle
			{
				alignment = TextAnchor.MiddleCenter,
				normal =
				{
					textColor = Color.white
				},
				fontStyle = FontStyle.Bold,
				fontSize = 10
			};
			return s;
		});
	}
}

#endif