// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BoxCommentAttribute))]
	internal class BoxComment_ : __DecoratorDrawer<BoxCommentAttribute>
	{
		public static class CFG
		{
			public const int FONT_SIZE = 12;
			public const FontStyle FONT_STYLE = FontStyle.Italic;
		}

		protected override float GetHeight(in float w)
		{
			return _style.CalcHeight(_label, w) + 0f;
		}

		protected override (byte, byte, byte, byte) GetPadding()
		{
			var d = base.GetPadding();
			d.Item1 = 15;
			return d;
		}

		protected override void OnInit()
		{
			_style = CreateStyle();
			_label = new GUIContent(_Attribute.Text);
		}

		protected override void OnBackground(in Rect pos)
		{
			var c = _Attribute.BackgroundColor * _Attribute.BoxOpacity;
			var leftBorder = pos;
			leftBorder.width = 3f;
			EditorGUI.DrawRect(leftBorder, _Attribute.BackgroundColor);
			EditorGUI.DrawRect(pos, c);
		}

		protected override void OnContent(in Rect pos)
		{
			DrawText(pos, _label, _style, _Attribute.TextColor);
		}

		private GUIContent _label = null;
		private GUIStyle _style = null;

		private static GUIStyle CreateStyle()
		{
			var s = new GUIStyle(EditorStyles.wordWrappedLabel);
			s.fontSize = CFG.FONT_SIZE;
			s.fontStyle = CFG.FONT_STYLE;
			s.padding = new RectOffset();
			s.margin = new RectOffset();
			s.contentOffset = default;
			return s;
		}

	}
}