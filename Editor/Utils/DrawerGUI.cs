// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Reflection;

	internal static class DrawerGUI
	{
		public const float INDENT_W = 15f;

		public static void DrawTex(Texture tex, in Rect area)
		{
			DrawTex(tex, area, new Rect(0,0,1,1), Color.white);
		}

		public static void DrawTex(Texture tex, in Rect area, in Rect coords, Color color)
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

		public static bool PointerButton(in Rect pos)
		{
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			return GUI.Button(pos, GUIContent.none, GUIStyle.none);
		}

		public static void IndentRect(ref Rect r, int indentLevel)
		{
			r.SliceLeft(INDENT_W * indentLevel);
		}
		
		public static bool PopupButton(in Rect pos, in GUIContent label)
		{
			return EditorGUI.DropdownButton(pos, label, FocusType.Keyboard);
		}
		public static void PrefixLabel(ref Rect pos, GUIContent l, FieldInfo fo)
		{
			if (l == null || l == GUIContent.none || fo.IsArray())
			{
				return;
			}
			pos = EditorGUI.PrefixLabel(pos, l);
		}

		public static void MutedInfo(in Rect pos, string msg, MessageType type = MessageType.Error)
		{
			var bgColor = Color.clear;
			
			if (type == MessageType.Error)
			{
				bgColor = Color.red * 0.2f;
			}
			else if (type == MessageType.Warning)
			{
				bgColor = Color.yellow * 0.2f;
			}
			else if (type == MessageType.Info)
			{
				bgColor = Color.cyan * 0.2f;
			}
			GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);
			EditorGUI.DrawRect(pos, bgColor);
			GUI.Label(pos, msg, EditorStyles.centeredGreyMiniLabel);
		}

		public static void DragDrop<T>(Rect area, T arg, Action<T, UnityEngine.Object[]> onDrop, Action onMouseUp)
		{
			Event ev = Event.current;
	
			if (!area.Contains(ev.mousePosition))
			{
				return;
			}

			if (DragAndDrop.objectReferences.Length == 0)
			{
				return;
			}

			var hoverColor = PickSkin(Color.white.Fade(0.1f), Color.black.Fade(0.1f));

			EditorGUI.DrawRect(area, hoverColor);
			
			if (onMouseUp != null && ev.type == EventType.MouseUp)
			{
				onMouseUp.Invoke();
			}
			if (onDrop != null)
			{
				if (ev.type is EventType.DragUpdated or EventType.DragPerform)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					if (ev.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						onDrop?.Invoke(arg, DragAndDrop.objectReferences);
					}
					Event.current.Use();
				}
			}
		}
	}
}

#endif