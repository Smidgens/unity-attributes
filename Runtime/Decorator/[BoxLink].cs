// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	public sealed class BoxLinkAttribute : __BaseDecorator
	{
		public BoxLinkAttribute(string text, string url)
		{
			// order = -0;
			URL = url ?? string.Empty;
			this.text = text;
		}

		public BoxLinkAttribute(string url) : this(url,url)
		{
		}

		internal string URL { get; }
		internal string text { get; }
		internal Color color { get; } = Color.white;
		internal Color backgroundColor  { get; } = Color.white;
	}
}


#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(BoxLinkAttribute))]
	internal sealed class _BoxLinkAttribute : __DecoratorDrawer<BoxLinkAttribute>
	{
		protected override float GetHeight(in float w)
		{
			var h = _style.CalcHeight(_label, w - ICO_W);
			return Mathf.Max(h, ICO_W);
		}

		protected override void OnContent(in Rect p)
		{
			var pos = p;

			var tCOlor = GUI.backgroundColor;
			GUI.backgroundColor = _style.normal.textColor * 0.7f;
			GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);
			GUI.backgroundColor = tCOlor;

			var icoRect = pos.SliceLeft(ICO_W);
			icoRect.height = icoRect.width;
			icoRect = icoRect.Resized(-pos.height * 0.2f);

			PluginAtlas.DrawIcon(icoRect, EAtlasIcon.Link, _style.normal.textColor);

			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			if (GUI.Button(pos, string.Empty, GUIStyle.none))
			{
				Application.OpenURL(_Attribute.URL);
			}
			DrawText(pos, _label, _style, _Attribute.color);
		}

		protected override void OnInit()
		{
			_style = CreateStyle();
			_label = new GUIContent(_Attribute.text, _Attribute.URL);
		}

		private GUIContent _label;
		private GUIStyle _style;
		
		private static readonly float ICO_W = EditorGUIUtility.singleLineHeight * 1.5f;

		private static GUIStyle CreateStyle()
		{
			var s = new GUIStyle(EditorStyles.linkLabel)
			{
				fontSize = EditorStyles.miniLabel.fontSize,
				alignment = TextAnchor.MiddleLeft,
				padding = new RectOffset(2,4,2,2)
			};
			return s;
		}

	}
}

#endif