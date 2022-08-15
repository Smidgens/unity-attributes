// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	[CustomPropertyDrawer(typeof(BoxCommentAttribute))]
	internal class Info_ : DecoratorDrawer
	{
		public static class CFG
		{
			public const float M_TOP = 0f;
			public const float M_BOTTOM = 5f;
			public const float PADDING = 5f;
		}

		public override float GetHeight()
		{
			var w = Screen.width - 36f;
			var a = (BoxCommentAttribute)attribute;
			var h = _TEXT_STYLE.Value.CalcHeight(new GUIContent(a.Text), w);
			return h + (CFG.PADDING * 2f) + CFG.M_TOP + CFG.M_BOTTOM;
		}

		public override void OnGUI(Rect pos)
		{
			var a = (BoxCommentAttribute)attribute;
			var areaRect = pos;
			areaRect.position += new Vector2(0f, CFG.M_TOP);
			areaRect.height -= CFG.M_TOP + CFG.M_BOTTOM;
			EditorGUI.DrawRect(areaRect, Color.white * 0.3f);
			var textRect = areaRect.Resize(-5f); // padding
			EditorGUI.LabelField(textRect, a.Text, _TEXT_STYLE.Value);
		}

		private Lazy<GUIStyle> _TEXT_STYLE = new Lazy<GUIStyle>(() =>
		{
			var s = new GUIStyle(EditorStyles.wordWrappedLabel);
			s.fontSize = 12;
			s.contentOffset = default;
			s.fontStyle = FontStyle.Italic;
			return s;
		});

	}
}