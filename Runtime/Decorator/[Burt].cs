// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Burt
	/// </summary>
	public sealed class BurtAttribute : __BaseDecorator
	{
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using UnityEngine.Windows;

	[CustomPropertyDrawer(typeof(BurtAttribute))]
	internal sealed class _BurtAttribute : __DecoratorDrawer<BurtAttribute>
	{
		protected override float GetHeight(in float w)
		{
			if (!_tex)
			{
				_tex = GetBurt();
			}
			return (_tex.height / (float)_tex.width) * w * 0.5f;
		}

		private static Texture2D _tex;

		protected override void OnContent(in Rect p)
		{
			GUI.DrawTexture(p, _tex, ScaleMode.StretchToFill);
		}

		private static Texture2D GetBurt()
		{
			var pFolderPath = AssetDatabase.GUIDToAssetPath("0159e8201da476b4fa62de80358d5c81");
			pFolderPath = pFolderPath[..^13];
			var rootPath = Application.dataPath;
			if (Application.isEditor)
			{
				rootPath = rootPath[..^7];
			}
			var tex = new Texture2D(2,2);
			tex.LoadImage(File.ReadAllBytes($"{rootPath}/{pFolderPath}/.github/misc/burt.jpg"));
			tex.filterMode = FilterMode.Point;
			tex.Apply();
			return tex;
		}
	}
}

#endif