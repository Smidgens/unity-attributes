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
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BoxHeaderAttribute))]
	internal sealed class _BoxHeaderAttribute : __DecoratorDrawer<BoxHeaderAttribute>
	{
		protected override void OnInit()
		{
			_style = CreateStyle();
			_label = new GUIContent(_Attribute.text);
		}

		protected override float GetHeight(in float w)
		{
			return _style.CalcHeight(_label, w) + 0f;
		}

		protected override void OnContent(in Rect pos)
		{
			GUI.Box(pos, GUIContent.none);
			_style.alignment = _Attribute.alignment;
			_style.fontStyle = _Attribute.fontStyle;
			DrawText(pos, _label, _style, Color.white);
		}

		private GUIContent _label;
		private GUIStyle _style;

		private static GUIStyle CreateStyle()
		{
			return new GUIStyle(EditorStyles.largeLabel)
			{
				alignment = TextAnchor.MiddleCenter,
				padding = new RectOffset(7,7,7,7),
				wordWrap = true
			};
		}

	}
}

#endif