// smidgens @ github

#pragma warning disable 0414

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	// indices to editor icons in texture atlas
	internal enum EAtlasIcon
	{
		// row 0
		Close,
		ArrowLeft,
		ArrowRight,
		Folder,
		File,
		LinkExternal,
		Link,
		
		// row 1
		CurlyBrackets = 8,
		SquareBrackets,
		AngleBrackets,
		Parentheses,
		Hash,
		Primitive,
		Enum,
		
		// Row 2
		Comment = 16,
		Info,
		Warning,
		Error,

		// Class,
		// Delegate,
		// Static,
		// Attribute,
		// Exception,
		// Interface,
		// Struct,
		
		// Row 5
		BoxRounded = 40,
		
		// misc
		SwitchOn,
		SwitchOff,
	}


	[System.Serializable]
	internal static class PluginAtlas
	{
		private const int _TILE_COUNT = 8;
		private const float _TILE_SIZE = 1f / _TILE_COUNT;

		public static void DrawIcon(in Rect pos, EAtlasIcon icon, Color c)
		{
			DrawerGUI.DrawTex(_TEX_ATLAS.Value, pos, GetIconCoords(icon), c);
		}

		private const string _ATLAS_GUID = "e769e4d9f339626498a12b64168231ee";
	
		private static readonly System.Lazy<Texture2D> _TEX_ATLAS = new (() =>
		{
			var path = AssetDatabase.GUIDToAssetPath(_ATLAS_GUID);
			return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		});

		private static Rect GetIconCoords(EAtlasIcon icon)
		{
			// special case for switch textures as they take up two columns
			if (icon == EAtlasIcon.SwitchOff)
			{
				return new Rect(0, 0.75f, 0.25f, 0.125f);
			}
			if (icon == EAtlasIcon.SwitchOn)
			{
				return new Rect(0, 0.75f + 0.125f, 0.25f, 0.125f);
			}

			var i = (int)icon;
			
			int y = i / _TILE_COUNT, x = i % _TILE_COUNT;
			var offset = new Vector2(x, y) * _TILE_SIZE;

			return new Rect(new Vector2(x, y) * _TILE_SIZE, Vector2.one * _TILE_SIZE);

			// // could be changed to an array that corresponds to enum values
			// return icon switch
			// {
			// 	EAtlasIcon.Close => new Rect(0.25f, 0, 0.125f, 0.125f),
			// 	EAtlasIcon.ArrowLeft => new Rect(0.375f, 0, 0.125f, 0.125f),
			// 	EAtlasIcon.ArrowRight => new Rect(0.5f, 0, 0.125f, 0.125f),
			// 	EAtlasIcon.Folder => new Rect(0.625f, 0f, 0.125f, 0.125f),
			// 	EAtlasIcon.File => new Rect(0.75f, 0f, 0.125f, 0.125f),
			// 	EAtlasIcon.Delegate => new Rect(0, 0.25f, 0.125f, 0.125f),
			// 	EAtlasIcon.Static => new Rect(0.125f, 0.25f, 0.125f, 0.125f),
			// 	EAtlasIcon.Primitive => new Rect(0.25f, 0.25f, 0.125f, 0.125f),
			// 	EAtlasIcon.Attribute => new Rect(0, 0.375f, 0.125f, 0.125f),
			// 	EAtlasIcon.Exception => new Rect(0.125f, 0.375f, 0.125f, 0.125f),
			// 	EAtlasIcon.Enum => new Rect(0.25f, 0.375f, 0.125f, 0.125f),
			// 	EAtlasIcon.Interface => new Rect(0, 0.5f, 0.125f, 0.125f),
			// 	EAtlasIcon.Struct => new Rect(0.125f, 0.5f, 0.125f, 0.125f),
			// 	EAtlasIcon.Class => new Rect(0.25f, 0.5f, 0.125f, 0.125f),
			// 	EAtlasIcon.Link => new Rect(0.625f, 0.375f, 0.125f, 0.125f),
			// 	EAtlasIcon.LinkExternal => new Rect(0.625f, 0.25f, 0.125f, 0.125f),
			// 	// alert icons
			// 	EAtlasIcon.Comment => new Rect(0.75f, 0.25f, 0.125f, 0.125f),
			// 	EAtlasIcon.Info => new Rect(0.875f, 0.25f, 0.125f, 0.125f),
			// 	EAtlasIcon.Warning => new Rect(0.75f, 0.375f, 0.125f, 0.125f),
			// 	EAtlasIcon.Error => new Rect(0.875f, 0.375f, 0.125f, 0.125f),
			// 	//
			// 	EAtlasIcon.CurlyBrackets => new Rect(0.5f, 0.25f, 0.125f, 0.125f),
			// 	EAtlasIcon.BoxRounded => new Rect(0, 0.625f, 0.125f, 0.125f),
			// 	_ => default
			// };
		}
	}
}

#endif