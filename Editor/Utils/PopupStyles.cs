// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	internal static class PopupStyles
	{
		public static GUIStyle HeaderLabel => _HEADER_STYLE.Value;
		public static GUIStyle ItemLabel => _ITEM_STYLE.Value;

		private static readonly Lazy<GUIStyle> _HEADER_STYLE = new (() =>
		{
			var s = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
			{
				normal =
				{
					textColor = Color.white
				},
				fontStyle = FontStyle.Bold
			};
			return s;
		});

		private static readonly Lazy<GUIStyle> _ITEM_STYLE = new (() =>
		{
			var s = new GUIStyle(EditorStyles.miniLabel)
			{
				normal =
				{
					textColor = Color.white
				},
				fontStyle = FontStyle.Bold
			};
			return s;
		});
	}
}

#endif