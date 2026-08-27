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
			var path = AssetDatabase.GUIDToAssetPath("d485ff05e343d9e4397508903c34a430");
			var a = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
			var tex = new Texture2D(2,2);
			tex.LoadImage(a.bytes);
			tex.filterMode = FilterMode.Point;
			tex.Apply();
			return tex;
		}
	}
}

#endif