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
		ArrowLeft,
		ArrowRight,
		Close,
		SwitchOn,
		SwitchOff,
		Class,
		Delegate,
		Static,
		Primitive,
		Attribute,
		Exception,
		Enum,
		Interface,
		Struct,
		Link,
		
		
		Comment,
		Info,
		Warning,
		Error,
		
		
		Folder,
		File,
	}

	[System.Serializable]
	internal static class PluginAtlas
	{
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
			// could be changed to an array that corresponds to enum values
			return icon switch
			{
				EAtlasIcon.ArrowLeft => new Rect(0.5f, 0, 0.25f, 0.25f),
				EAtlasIcon.ArrowRight => new Rect(0.75f, 0, 0.25f, 0.25f),
				EAtlasIcon.SwitchOff => new Rect(0, 0, 0.25f, 0.125f),
				EAtlasIcon.SwitchOn => new Rect(0, 0.125f, 0.25f, 0.125f),
				EAtlasIcon.Close => new Rect(0.25f, 0, 0.25f, 0.25f),
				EAtlasIcon.Delegate => new Rect(0, 0.25f, 0.125f, 0.125f),
				EAtlasIcon.Static => new Rect(0.125f, 0.25f, 0.125f, 0.125f),
				EAtlasIcon.Primitive => new Rect(0.25f, 0.25f, 0.125f, 0.125f),
				EAtlasIcon.Attribute => new Rect(0, 0.375f, 0.125f, 0.125f),
				EAtlasIcon.Exception => new Rect(0.125f, 0.375f, 0.125f, 0.125f),
				EAtlasIcon.Enum => new Rect(0.25f, 0.375f, 0.125f, 0.125f),
				EAtlasIcon.Interface => new Rect(0, 0.5f, 0.125f, 0.125f),
				EAtlasIcon.Struct => new Rect(0.125f, 0.5f, 0.125f, 0.125f),
				EAtlasIcon.Class => new Rect(0.25f, 0.5f, 0.125f, 0.125f),
				EAtlasIcon.Link => new Rect(0.5f, 0.25f, 0.25f, 0.25f),
				//
				EAtlasIcon.Comment => new Rect(0.75f, 0.25f, 0.25f, 0.25f),
				EAtlasIcon.Info => new Rect(0.75f, 0.25f, 0.25f, 0.25f),
				EAtlasIcon.Warning => new Rect(0.75f, 0.25f, 0.25f, 0.25f),
				EAtlasIcon.Error => new Rect(0.75f, 0.25f, 0.25f, 0.25f),
				//
				EAtlasIcon.Folder => new Rect(0.5f, 0.5f, 0.25f, 0.25f),
				EAtlasIcon.File => new Rect(0.75f, 0.5f, 0.25f, 0.25f),
				_ => default
			};
		}
	}
}

#endif