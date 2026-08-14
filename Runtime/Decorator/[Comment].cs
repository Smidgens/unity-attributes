// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	public enum ECommentType
	{
		Generic,
		Info,
		Warning,
		Error
	}

	/// <summary>
	/// Draws comment paragraph with customizable colors and background
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class CommentAttribute : __BaseDecorator
	{
		public CommentAttribute
		(
			string text,
			ECommentType type = ECommentType.Generic,
			string color = null,
			string bgColor = null
		)
		{
			this.text = text ?? string.Empty;
			this.color = ParseColor(color, this.color);
			backgroundColor = ParseColor(bgColor, backgroundColor);
			this.commentType = type;
		}

		internal ECommentType commentType { get; }
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
			var h = s.CalcHeight(_label, w - ICO_W);
			return Mathf.Max(h, ICO_W);
		}

		private static readonly float ICO_W = EditorGUIUtility.singleLineHeight * 1.5f;

		protected override void OnInit()
		{
			_label = new GUIContent(_Attribute.text);
		}

		protected override void OnContent(in Rect p)
		{
			var s = DrawerStyles.ParagraphSM;
			var pos = p;
			var tCOlor = GUI.backgroundColor;
			GUI.backgroundColor = s.normal.textColor * 0.7f;
			GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);
			GUI.backgroundColor = tCOlor;

			var tintColor = GetTintColor(_Attribute.commentType);
			EditorGUI.DrawRect(pos, tintColor * 0.2f);

			var icoRect = pos.SliceLeft(ICO_W);
			icoRect.height = icoRect.width;
			icoRect = icoRect.Resized(-pos.height * 0.2f);

			PluginAtlas.DrawIcon(icoRect, GetIcon(_Attribute.commentType), s.normal.textColor);
			DrawText(pos, _label, s, _Attribute.color);
		}

		private static Color GetTintColor(ECommentType t)
		{
			return t switch
			{
				ECommentType.Info => Color.cyan,
				ECommentType.Warning => Color.yellow,
				ECommentType.Error => Color.red,
				_ => Color.clear
			};
		}

		private static EAtlasIcon GetIcon(ECommentType t)
		{
			return t switch
			{
				ECommentType.Info => EAtlasIcon.Info,
				ECommentType.Warning => EAtlasIcon.Warning,
				ECommentType.Error => EAtlasIcon.Error,
				_ => EAtlasIcon.Comment,
			};
		}
		
		private GUIContent _label;

		

	}
}

#endif