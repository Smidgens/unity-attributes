// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Draws comment paragraph with customizable colors and background
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class CommentAttribute : __BaseDecorator
	{
		public CommentAttribute
		(
			string text
		)
		{
			this.text = text ?? string.Empty;
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

	[CustomPropertyDrawer(typeof(CommentAttribute))]
	internal sealed class _BoxCommentAttribute : __DecoratorDrawer<CommentAttribute>
	{
		protected override float GetHeight(in float w)
		{
			var s = DrawerStyles.ParagraphSM;
			var h = s.CalcHeight(_label, w - _ICO_W);
			return Mathf.Max(h, _ICO_W);
		}

		private static readonly float _ICO_W = EditorGUIUtility.singleLineHeight * 1.5f;
		private static readonly float _ICON_OPACITY = DrawerGUI.PickSkin(1f, 0.75f);
		
		protected override void OnInit()
		{
			_label = new GUIContent(_Attribute.text);
		}

		protected override void OnContent(in Rect p)
		{
			var s = DrawerStyles.ParagraphSM;
			var pos = p;

			var box = p;
			box.width -= 2f;
			box.center = p.center;

			GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);
			var icoRect = pos.SliceLeft(_ICO_W);
			icoRect.height = icoRect.width;
			icoRect = icoRect.Resized(-pos.height * 0.3f);
			var icoColor = s.normal.textColor.Fade(_ICON_OPACITY);
			PluginAtlas.DrawIcon(icoRect, EAtlasIcon.Comment, icoColor);
			GUI.Label(pos, _label, s);
		}
		
		private GUIContent _label;

		

	}
}

#endif