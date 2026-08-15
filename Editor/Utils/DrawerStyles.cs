// smidgens @ github

#pragma warning disable 0414

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// Styles used across drawers
	/// </summary>
	internal static class DrawerStyles
	{
		public ref struct StyleInfo
		{
			public StyleInfo(GUIStyle s, float h)
			{
				style = s;
				height = h;
			}
			public readonly GUIStyle style;
			public readonly float height;
		}

		public static GUIStyle ButtonSM => _BTN_SM_STYLE.Value;
		public static GUIStyle LabelLG => _LABEL_LG.Value;
		public static GUIStyle Foldout => _FOLDOUT.Value;
		public static GUIStyle FoldoutLG => _FOLDOUT_LG.Value;
		public static GUIStyle ParagraphSM => _PARAGRAPH_SM.Value;
		public static GUIStyle ParagraphBoldSM => _PARAGRAPH_SM_BOLD.Value;

		// 
		public static GUIStyle LabelSM => _LABEL_SM.Value;
		public static GUIStyle LabelLink => _LABEL_LINK.Value;
		public static float LabelHeightLink => _LABEL_LINK_H.Value;
		public static float LabelHeightLG => _LABEL_LG_H.Value;
		public static float FoldoutHeight => _FOLDOUT_H.Value;
		public static float FoldoutHeightLG => _FOLDOUT_LG_H.Value;
		public static float ButtonHeightSM => _BTN_SM_H.Value;

		private static readonly Lazy<GUIStyle> _BTN_SM_STYLE = new(() => new GUIStyle(EditorStyles.miniButton)
		{
			fontSize = (int)(EditorStyles.miniButton.fontSize * 0.9f)
		});
		
		private static readonly Lazy<GUIStyle> _FOLDOUT = new(() => new GUIStyle(EditorStyles.foldout)
		{

		});
		
		private static readonly Lazy<GUIStyle> _FOLDOUT_LG = new(() => new GUIStyle(EditorStyles.foldout)
		{
			fontSize = (int)(EditorStyles.foldout.fontSize * 1.2f)
		});

		private static readonly Lazy<GUIStyle> _LABEL_SM = new(() => new GUIStyle(EditorStyles.miniLabel)
		{
			alignment = TextAnchor.MiddleRight
		});

		private static readonly Lazy<GUIStyle> _LABEL_LG = new(() => new GUIStyle(EditorStyles.largeLabel)
		{
			alignment = TextAnchor.MiddleCenter,
			padding = new RectOffset(7,7,7,7),
			wordWrap = true
		});
		
		private static readonly Lazy<GUIStyle> _PARAGRAPH_SM = new(() => new GUIStyle(EditorStyles.wordWrappedLabel)
		{
			fontSize = EditorStyles.miniLabel.fontSize,
			alignment = TextAnchor.MiddleLeft,
			padding = new RectOffset(2,2,2,2)
		});
		
		private static readonly Lazy<GUIStyle> _PARAGRAPH_SM_BOLD = new(() => new GUIStyle(_PARAGRAPH_SM.Value)
		{
			fontStyle = FontStyle.Bold
		});
		
		private static readonly Lazy<GUIStyle> _LABEL_LINK = new(() => new GUIStyle(EditorStyles.linkLabel)
		{
			fontSize = EditorStyles.miniLabel.fontSize,
			alignment = TextAnchor.MiddleLeft,
			padding = new RectOffset(2,2,2,2)
		});

		private static readonly Lazy<float> _LABEL_LINK_H = new (() => _LABEL_LINK.Value.CalcHeight(GUIContent.none, 50));
		private static readonly Lazy<float> _BTN_SM_H = new (() => _BTN_SM_STYLE.Value.CalcHeight(GUIContent.none, 50));
		private static readonly Lazy<float> _LABEL_LG_H = new (() => _LABEL_LG.Value.CalcHeight(GUIContent.none, 50));
		private static readonly Lazy<float> _FOLDOUT_H = new (() => _FOLDOUT.Value.CalcHeight(GUIContent.none, 50));
		private static readonly Lazy<float> _FOLDOUT_LG_H = new (() => _FOLDOUT_LG.Value.CalcHeight(GUIContent.none, 50));

	}
}

#endif