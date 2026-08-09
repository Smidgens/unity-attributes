// // smidgens @ github
//
// #if UNITY_EDITOR
//
// namespace Smidgenomics.Unity.Attributes.Editor
// {
// 	using UnityEngine;
// 	using UnityEditor;
// 	using System;
//
// 	internal static class PopupStyles
// 	{
// 		public static GUIStyle HeaderLabel => _HEADER_STYLE.Value;
// 		public static GUIStyle ItemLabel => _ITEM_STYLE.Value;
// 		public static float ScrollbarWidth => _SCROLLBAR_W.Value;
//
// 		public static float HeaderHeight => _HEADER_STYLE.Value.CalcHeight(GUIContent.none, 100);
// 		public static float ItemHeight => ItemLabel.CalcHeight(GUIContent.none, 100);
//
// 		private static readonly Lazy<float> _SCROLLBAR_W =
// 		new(() => GUI.skin.verticalScrollbar.CalcSize(GUIContent.none).x);
// 		
// 		private static readonly Lazy<GUIStyle> _ITEM_STYLE =
// 		new (() => new GUIStyle(EditorStyles.miniLabel)
// 		{
// 			padding = new RectOffset(6,3,3,3)
// 		});
//
// 		private static readonly Lazy<GUIStyle> _HEADER_STYLE = new (() => new GUIStyle(EditorStyles.label)
// 		{
// 			fontStyle = FontStyle.Bold,
// 			alignment = TextAnchor.MiddleCenter,
// 			padding = new RectOffset(5,5,7,7)
// 		});
// 	}
// }
//
// #endif