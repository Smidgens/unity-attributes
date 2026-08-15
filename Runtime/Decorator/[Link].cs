// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	/// <summary>
	/// Draws link
	/// </summary>
	public sealed class LinkAttribute : __BaseDecorator
	{
		public LinkAttribute(string text, string url)
		{
			// order = -0;
			URL = url ?? string.Empty;
			this.text = text;
		}

		public LinkAttribute(string url) : this(url,url)
		{
		}

		internal string URL { get; }
		internal string text { get; }
	}
}


#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(LinkAttribute))]
	internal sealed class _LinkAttribute : __DecoratorDrawer<LinkAttribute>
	{
		protected override float GetHeight(in float w)
		{
			var h = DrawerStyles.LabelLink.CalcHeight(_label, w - ICO_W);
			return Mathf.Max(h, ICO_W);
		}

		protected override void OnContent(in Rect p)
		{
			var pos = p;

			GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);
			var icoRect = pos.SliceLeft(ICO_W);
			icoRect.height = icoRect.width;
			icoRect = icoRect.Resized(-pos.height * 0.25f);

			PluginAtlas.DrawIcon(icoRect, EAtlasIcon.Link, DrawerStyles.LabelLink.normal.textColor);

			var lSize = DrawerStyles.LabelLink.CalcSize(_label);

			var linkRect = new Rect(pos.position, lSize);
			
			EditorGUIUtility.AddCursorRect(linkRect, MouseCursor.Link);
			if (GUI.Button(linkRect, string.Empty, GUIStyle.none))
			{
				Application.OpenURL(_Attribute.URL);
			}
			GUI.Label(pos, _label, DrawerStyles.LabelLink);
		}

		protected override void OnInit()
		{
			_label = new GUIContent(_Attribute.text, _Attribute.URL);
		}

		private GUIContent _label;
		
		private static readonly float ICO_W = EditorGUIUtility.singleLineHeight * 1.5f;

		

	}
}

#endif