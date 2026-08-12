// smidgens @ github

// resharper disable all

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	internal abstract class __DecoratorDrawer<T> : DecoratorDrawer where T : __BaseDecorator
	{
		protected T _Attribute => attribute as T;

		private static readonly float _MARGIN = EditorGUIUtility.standardVerticalSpacing * 1.5f;

		public sealed override float GetHeight()
		{
			if (!_init)
			{
				OnInit();
				_init = true;
			}
			return GetHeight( Screen.width) + _MARGIN;
		}

		public sealed override void OnGUI(Rect pos)
		{
			pos.height -= _MARGIN;
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