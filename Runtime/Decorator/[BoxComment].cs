// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class BoxCommentAttribute : __BaseDecorator
	{
		public BoxCommentAttribute
		(
			string text,
			string color = null,
			string bgColor = null
		)
		{
			this.text = text ?? string.Empty;
			this.color = Parse(color, this.color);
			backgroundColor = Parse(bgColor, backgroundColor);
		}

		internal string text { get; }
		internal Color color { get; } = Color.white;
		internal Color backgroundColor  { get; } = Color.white;
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BoxCommentAttribute))]
	internal sealed class _BoxCommentAttribute : __DecoratorDrawer<BoxCommentAttribute>
	{
		protected override float GetHeight(in float w)
		{
			var h = _style.CalcHeight(_label, w - ICO_W);
			return Mathf.Max(h, ICO_W);
		}

		private static readonly float ICO_W = EditorGUIUtility.singleLineHeight * 1.5f;

		protected override void OnInit()
		{
			_style = CreateStyle();
			_label = new GUIContent(_Attribute.text);
		}

		protected override void OnContent(in Rect p)
		{
			var pos = p;
			var tCOlor = GUI.backgroundColor;
			GUI.backgroundColor = _style.normal.textColor * 0.7f;
			GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);
			GUI.backgroundColor = tCOlor;
			var icoRect = pos.SliceLeft(ICO_W);
			icoRect.height = icoRect.width;
			icoRect = icoRect.Resized(-pos.height * 0.2f);
			PluginAtlas.DrawIcon(icoRect, EAtlasIcon.Comment, _style.normal.textColor);
			DrawText(pos, _label, _style, _Attribute.color);
		}

		private GUIContent _label;
		private GUIStyle _style;

		private static GUIStyle CreateStyle()
		{
			return new GUIStyle(EditorStyles.wordWrappedLabel)
			{
				fontSize = EditorStyles.miniLabel.fontSize,
				alignment = TextAnchor.MiddleLeft,
				padding = new RectOffset(2,4,4,4)
			};
		}

	}
}

#endif