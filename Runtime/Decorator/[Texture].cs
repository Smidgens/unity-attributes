// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draws image texture above field
	/// </summary>
	public sealed class TextureAttribute : __BaseDecorator
	{
		public TextureAttribute
		(
			string guid
		)
		{
			this.guid = guid;
		}

		internal string guid { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(TextureAttribute))]
	internal sealed class _TextureAttribute : __DecoratorDrawer<TextureAttribute>
	{
		protected override float GetHeight(in float w)
		{
			if (!_tex.Item1)
			{
				var path = AssetDatabase.GUIDToAssetPath(_Attribute.guid);
				_tex = (true, AssetDatabase.LoadAssetAtPath<Texture>(path));
			}
			if (!_tex.Item2)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			var tex = _tex.Item2;
			var ratio = tex.height / (float)tex.width;
			return ratio * w * 0.5f;
		}

		private (bool, Texture) _tex;

		protected override void OnContent(in Rect p)
		{
			if (_tex.Item2)
			{
				GUI.DrawTexture(p, _tex.Item2, ScaleMode.StretchToFill);
			}
			else
			{
				EditorGUI.DrawRect(p, Color.red * 0.3f);
			}
		}
	}
}

#endif