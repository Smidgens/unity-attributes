// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	public sealed class BoxHeaderAttribute : __BaseDecorator
	{
		public BoxHeaderAttribute
		(
			string text,
			string textColor = null,
			string bgColor = null,
			TextAnchor alignment = TextAnchor.MiddleLeft,
			FontStyle fontStyle = FontStyle.Bold
		)
		{
			this.text = text ?? string.Empty;
			color = ParseColor(textColor, color);
			backgroundColor = ParseColor(bgColor, backgroundColor);
			this.alignment = alignment;
			this.fontStyle = fontStyle;
		}
		
		internal string text { get; }
		internal TextAnchor alignment { get; }
		internal FontStyle fontStyle { get; }
		internal Color color { get; } = Color.white;
		internal Color backgroundColor  { get; } = Color.white;
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BoxHeaderAttribute))]
	internal sealed class _BoxHeaderAttribute : __DecoratorDrawer<BoxHeaderAttribute>
	{
		protected override void OnInit()
		{
			_label.text = _Attribute.text;
		}

		protected override float GetHeight(in float w)
		{
			return DrawerStyles.LabelHeightLG;
		}

		protected override void OnContent(in Rect pos)
		{
			GUI.Box(pos, GUIContent.none);

			var s = DrawerStyles.LabelLG;
			var tAlignment = s.alignment;
			var tStyle = s.fontStyle;
			s.alignment = _Attribute.alignment;
			s.fontStyle = _Attribute.fontStyle;
			DrawText(pos, _label, s, Color.white);
			s.alignment = tAlignment;
			s.fontStyle = tStyle;
		}

		private readonly GUIContent _label = new ();

	}
}

#endif