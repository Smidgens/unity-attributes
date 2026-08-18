// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Draws horizontal divider above field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class DividerAttribute : __BaseDecorator
	{
		public DividerAttribute
		(
			byte marginTop = 4,
			byte marginBottom = 5,
			string color = null
		)
		{
			this.marginTop = marginTop;
			this.marginBottom = marginBottom;
			if (!string.IsNullOrEmpty(color) && ColorUtility.TryParseHtmlString(color, out var c))
			{
				hasColor = true;
				this.color = c;
			}
		}

		internal float marginTop { get; }
		internal float marginBottom { get; }
		internal bool hasColor { get; }
		internal Color color { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DividerAttribute))]
	internal sealed class _DividerAttribute : __DecoratorDrawer<DividerAttribute>
	{
		protected override float GetHeight(in float w)
		{
			return _SEP_H + _Attribute.marginTop + _Attribute.marginBottom;
		}

		private const float _SEP_H = 1f;

		public static readonly Color SEP_COLOR =
		DrawerGUI.PickSkin(Color.white.Fade(0.1f), Color.black.Fade(0.1f));

		protected override void OnContent(in Rect p)
		{
			var pos = p;
			pos.SliceTop(_Attribute.marginTop);
			var sepRect = pos.SliceTop(_SEP_H);
			pos.SliceTop(_Attribute.marginBottom);
			var color = _Attribute.hasColor
			? _Attribute.color
			: SEP_COLOR;
			EditorGUI.DrawRect(sepRect, color);
		}
	}
}

#endif