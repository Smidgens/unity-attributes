// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(BoxLinkAttribute))]
	internal class BoxLink_ : __DecoratorDrawer<BoxLinkAttribute>
	{
		public static class CFG
		{
			public const int FONT_SIZE = 11;
			public const FontStyle FONT_STYLE = FontStyle.Normal;
		}

		protected override (byte, byte, byte, byte) GetPadding()
		{
			(byte, byte, byte, byte) p = (5, 5, 2, 2);
			p.Item1 = 20;
			return p;
		}

		protected override void OnBackground(in Rect pos)
		{
			EditorGUI.DrawRect(pos, Color.white * 0.5f);
			EditorGUI.DrawRect(pos, _style.normal.textColor * 0.4f);
			var leftBorder = pos;
			leftBorder.width = 3f;
			EditorGUI.DrawRect(leftBorder, _style.normal.textColor);
		}

		protected override float GetHeight(in float w)
		{
			return _style.CalcHeight(_label, w);
		}

		protected override void OnContent(in Rect pos)
		{
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			if (GUI.Button(pos, "", GUIStyle.none))
			{
				Application.OpenURL(_Attribute.URL);
			}
			DrawText(pos, _label, _style);
		}

		protected override void OnInit()
		{
			_style = CreateStyle();
			_label = new GUIContent(_Attribute.Text, _Attribute.URL);
		}

		private GUIContent _label = null;
		private GUIStyle _style = null;

		private static GUIStyle CreateStyle()
		{
			var s = new GUIStyle(EditorStyles.linkLabel);
			s.fontSize = CFG.FONT_SIZE;
			s.fontStyle = CFG.FONT_STYLE;
			s.padding = new RectOffset();
			s.margin = new RectOffset();
			s.contentOffset = default;
			return s;
		}

	}
}

#endif