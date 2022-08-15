// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;

	internal static class SpriteGUI
	{
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
