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
		public static GUIStyle ItemLabel => EditorStyles.miniLabel;

		private static readonly Lazy<GUIStyle> _HEADER_STYLE = new (() => new GUIStyle(EditorStyles.centeredGreyMiniLabel)
		{
			fontStyle = FontStyle.Bold
		});
	}
}

#endif