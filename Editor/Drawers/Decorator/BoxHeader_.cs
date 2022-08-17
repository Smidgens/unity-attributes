// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BoxHeaderAttribute))]
	internal class BoxHeader_ : __DecoratorDrawer<BoxHeaderAttribute>
	{
		public static class CFG
		{
			public const int FONT_SIZE = 13;
			public const FontStyle FONT_STYLE = FontStyle.Bold;
		}

		protected override void OnInit()
		{
			_style = CreateStyle();
			_label = new GUIContent(_Attribute.Text);
		}

		protected override float GetHeight(in float w)
		{
			return _style.CalcHeight(_label, w) + 0f;
		}

		protected override void OnBackground(in Rect pos)
		{
			EditorGUI.DrawRect(pos, Color.white);
			EditorGUI.DrawRect(pos, Color.black * 0.8f);
		}

		protected override void OnContent(in Rect pos)
		{
			DrawText(pos, _label, _style, Color.white);
		}

		private GUIContent _label = null;
		private GUIStyle _style = null;

		private static GUIStyle CreateStyle()
		{
			var s = new GUIStyle(EditorStyles.whiteLargeLabel);
			s.fontSize = CFG.FONT_SIZE;
			s.fontStyle = CFG.FONT_STYLE;
			s.alignment = TextAnchor.MiddleCenter;
			s.wordWrap = true;
			return s;
		}

	}
}