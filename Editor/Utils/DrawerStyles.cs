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
	internal sealed class DrawerStyles
	{
		private DrawerStyles(){}

		public static GUIStyle ButtonSM => GetInstance()._BTN_SM_STYLE.Value;
		
		public static GUIStyle Foldout => GetInstance()._FOLDOUT.Value;
		public static GUIStyle FoldoutLG => GetInstance()._FOLDOUT_LG.Value;
		public static GUIStyle ParagraphSM => GetInstance()._PARAGRAPH_SM.Value;
		public static GUIStyle ParagraphBoldSM => GetInstance()._PARAGRAPH_SM_BOLD.Value;
		public static GUIStyle TextArea => GetInstance()._TEXT_AREA.Value;
		public static GUIStyle LabelLG => GetInstance()._LABEL_LG.Value;
		public static GUIStyle LabelMD => GetInstance()._LABEL_MD.Value;
		public static GUIStyle LabelSM => GetInstance()._LABEL_SM.Value;
		public static GUIStyle LabelLink => GetInstance()._LABEL_LINK.Value;
		public static float LabelHeightLink => GetInstance()._LABEL_LINK_H.Value;
		public static float LabelHeightLG => GetInstance()._LABEL_LG_H.Value;
		public static float LabelHeightMD => GetInstance()._LABEL_MD_H.Value;
		public static float FoldoutHeight => GetInstance()._FOLDOUT_H.Value;
		public static float FoldoutHeightLG => GetInstance()._FOLDOUT_LG_H.Value;
		public static float ButtonHeightSM => GetInstance()._BTN_SM_H.Value;

		public static DrawerStyles GetInstance()
		{
			// domain reload nonsense
			if (_instance != null && _instance._LABEL_LG.Value == null)
			{
				_instance = null;
			}

			if (_instance == null)
			{
				_instance = new DrawerStyles();
			}

			return _instance;

		}
		
		private static DrawerStyles _instance;

		private readonly Lazy<GUIStyle> _BTN_SM_STYLE = new(() => new GUIStyle(EditorStyles.miniButton)
		{
			fontSize = (int)(EditorStyles.miniButton.fontSize * 0.9f)
		});
		
		private readonly Lazy<GUIStyle> _FOLDOUT = new(() => new GUIStyle(EditorStyles.foldout)
		{

		});
		
		private readonly Lazy<GUIStyle> _FOLDOUT_LG = new(() => new GUIStyle(EditorStyles.foldout)
		{
			fontSize = (int)(EditorStyles.foldout.fontSize * 1.2f)
		});
		
		private readonly Lazy<GUIStyle> _TEXT_AREA = new(() => new GUIStyle(EditorStyles.textArea)
		{
			// fontSize = (int)(EditorStyles.foldout.fontSize * 1.2f)
		});

		private readonly Lazy<GUIStyle> _LABEL_SM = new(() => new GUIStyle(EditorStyles.miniLabel)
		{
			// alignment = TextAnchor.MiddleRight
		});

		private readonly Lazy<GUIStyle> _LABEL_LG = new(() => new GUIStyle(EditorStyles.largeLabel)
		{
			alignment = TextAnchor.MiddleCenter,
			padding = new RectOffset(7,7,7,7),
			wordWrap = true
		});
		
		private readonly Lazy<GUIStyle> _LABEL_MD = new(() => new GUIStyle(EditorStyles.boldLabel)
		{
			// alignment = TextAnchor.MiddleCenter,
			// padding = new RectOffset(7,7,7,7),
			// wordWrap = true
		});
		
		private readonly Lazy<GUIStyle> _PARAGRAPH_SM = new(() => new GUIStyle(EditorStyles.wordWrappedLabel)
		{
			fontSize = EditorStyles.miniLabel.fontSize,
			alignment = TextAnchor.MiddleLeft,
			padding = new RectOffset(2,2,2,2)
		});
		
		private readonly Lazy<GUIStyle> _PARAGRAPH_SM_BOLD = new(() => new GUIStyle(EditorStyles.wordWrappedLabel)
		{
			fontSize = EditorStyles.miniLabel.fontSize,
			alignment = TextAnchor.MiddleLeft,
			padding = new RectOffset(2,2,2,2),
			fontStyle = FontStyle.Bold
		});
		
		private readonly Lazy<GUIStyle> _LABEL_LINK = new(() => new GUIStyle(EditorStyles.linkLabel)
		{
			fontSize = EditorStyles.miniLabel.fontSize,
			alignment = TextAnchor.MiddleLeft,
			padding = new RectOffset(2,2,2,2)
		});

		private readonly Lazy<float> _LABEL_LINK_H = new (() => LabelLink.CalcHeight(GUIContent.none, 50));
		private readonly Lazy<float> _BTN_SM_H = new (() => ButtonSM.CalcHeight(GUIContent.none, 50));
		private readonly Lazy<float> _LABEL_LG_H = new (() => LabelLG.CalcHeight(GUIContent.none, 50));
		private readonly Lazy<float> _LABEL_MD_H = new (() => LabelMD.CalcHeight(GUIContent.none, 50));
		private readonly Lazy<float> _FOLDOUT_H = new (() => Foldout.CalcHeight(GUIContent.none, 50));
		private readonly Lazy<float> _FOLDOUT_LG_H = new (() => FoldoutLG.CalcHeight(GUIContent.none, 50));

	}
}

#endif