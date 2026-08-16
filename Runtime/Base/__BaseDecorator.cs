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

		protected virtual (float, float) GetVerticalMargins()
		{
			var m = EditorGUIUtility.standardVerticalSpacing * 0.5f;
			return (m, m);
		}

		public sealed override float GetHeight()
		{
			if (!_init)
			{
				OnInit();
				_init = true;
			}

			var (tm, bm) = GetVerticalMargins();

			return GetHeight(Screen.width) + tm + bm;
		}

		public sealed override void OnGUI(Rect pos)
		{
			DrawerGUI.IndentRect(ref pos, EditorGUI.indentLevel);
			

			var (tm, bm) = GetVerticalMargins();
			pos.SliceTop(tm);
			pos.height -= bm;

			var tIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			OnContent(pos);
			EditorGUI.indentLevel = tIndent;
		}

		protected virtual void OnInit() { }
		protected virtual float GetHeight(in float w) => EditorGUIUtility.singleLineHeight;
		protected virtual void OnContent(in Rect pos) { }

		protected void DrawText(in Rect pos, GUIContent l, GUIStyle s, in Color tintColor)
		{
			// var tc = GUI.contentColor;
			var tc = s.normal.textColor;
			var tstyle = s.fontStyle;
			// s.fontStyle = FontStyle.Bold;
			// GUI.contentColor = tintColor;
			s.normal.textColor = tintColor;
			EditorGUI.LabelField(pos, l, s);
			s.normal.textColor = tc;
			s.fontStyle = tstyle;
			// GUI.contentColor = tc;
		}
		
		protected void DrawText(in Rect pos, GUIContent l, GUIStyle s)
		{
			EditorGUI.LabelField(pos, l, s);
		}

		private bool _init;

	}
}

#endif