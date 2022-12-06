// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	internal enum AtlasIcon
	{
		ArrowLeft,
		ArrowRight,
		Switch,
		Times,
	}

	internal static class SpriteGUI
	{
		public static void DrawIcon(in Rect pos, in AtlasIcon ico)
		{
			// todo: add actual icons to image atlas
			if (ico == AtlasIcon.ArrowLeft)
			{
				EditorGUI.LabelField(pos, "<·", PopupStyles.ItemLabel);
			}
			else if (ico == AtlasIcon.ArrowRight)
			{
				EditorGUI.LabelField(pos, "·>", PopupStyles.ItemLabel);
			}
		}

		public static void AtlasRow(in Rect pos, Texture atlas, in int rows, in int index)
		{
			var rowHeight = pos.width / rows;
			var yOffset = index * rowHeight;
			using (new GUI.ClipScope(pos))
			{
				var iconRect = pos;
				iconRect.height = iconRect.width;
				// sprite offset
				var spriteOffset = yOffset;
				iconRect.position = new Vector2(0f, -yOffset);
				GUI.DrawTexture(iconRect, atlas);
			}
		}


	}
}


#endif