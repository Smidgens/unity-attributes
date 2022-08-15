// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	[CustomPropertyDrawer(typeof(BoxHeaderAttribute))]
	internal class BoxHeader_ : DecoratorDrawer
	{
		public static class CFG
		{
			public const float HEIGHT = 20f;
			public const float SPACE_TOP = 10f;
			public const float SPACE_BOTTOM = 5f;
			public const float TOTAL_HEIGHT = HEIGHT + SPACE_TOP + SPACE_BOTTOM;
		}

		public override float GetHeight()
		{
			return CFG.TOTAL_HEIGHT;
		}

		public override void OnGUI(Rect pos)
		{
			pos.height = CFG.HEIGHT;
			pos.position += new Vector2(0, CFG.SPACE_TOP);
			var a = (BoxHeaderAttribute)attribute;
			GUI.Box(pos, "", GUI.skin.box);
			var lpos = pos;
			lpos.position += new Vector2(0f, 0f);
			EditorGUI.LabelField(lpos, a.Text, _HEADER_STYLE.Value);
		}

		private static Lazy<GUIStyle> _HEADER_STYLE = new Lazy<GUIStyle>(() =>
		{
			var s = new GUIStyle(EditorStyles.whiteLargeLabel);
			s.alignment = TextAnchor.MiddleCenter;
			s.normal.textColor = Color.white;
			s.fontStyle = FontStyle.Bold;
			return s;
		});

	}
}