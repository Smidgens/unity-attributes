// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public abstract class __BaseDecorator : __Base
	{
		protected __BaseDecorator() : base(true) {}

		protected static Color ParseColor(string c, in Color defaultValue)
		{
			if (ColorUtility.TryParseHtmlString(c ?? string.Empty, out var r))
			{
				return r;
			}
			return defaultValue;
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	internal abstract class __DecoratorDrawer<T> : DecoratorDrawer where T : __BaseDecorator
	{
		protected T _Attribute => attribute as T;

		public sealed override float GetHeight()
		{
			if (!_init)
			{
				OnInit();
				_init = true;
			}
			return GetHeight(Screen.width);
		}

		public sealed override void OnGUI(Rect pos)
		{
			// pos.height -= _MARGIN;
			OnContent(pos);
		}

		protected virtual void OnInit() { }
		protected virtual float GetHeight(in float w) => EditorGUIUtility.singleLineHeight;
		protected virtual void OnContent(in Rect pos) { }

		protected void DrawText(in Rect pos, GUIContent l, GUIStyle s, in Color c)
		{
			var tc = GUI.contentColor;
			GUI.contentColor = c;
			EditorGUI.LabelField(pos, l, s);
			GUI.contentColor = tc;
		}

		private bool _init;

	}
}

#endif