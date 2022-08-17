// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	using BoxOffset = System.ValueTuple<byte, byte, byte, byte>;

	internal abstract class __DecoratorDrawer<T> : DecoratorDrawer
	where T : __BaseDecorator
	{
		protected T _Attribute => attribute as T;

		protected virtual (byte, byte) GetMargin() => _Attribute.Margin;
		protected virtual (byte, byte, byte, byte) GetPadding() => _Attribute.Padding;

		public override sealed float GetHeight()
		{
			if (!_init)
			{
				OnInit();
				_init = true;
			}
			var (mt, mb) = GetMargin();
			var (_1, _2, pt, pb) = GetPadding();
			var spacing = 36f; // hacky magic value atm
			var totalWidth = Screen.width - spacing;
			var p2 = pt + pb;
			var contentWidth = totalWidth - p2;
			return
			GetHeight(contentWidth)
			+ p2 // padding
			+ (mt + mb); // y margin
		}

		public override sealed void OnGUI(Rect pos)
		{
			var (mt, mb) = GetMargin();
			var c = pos.center;
			pos.height -= mt + mb;
			pos.center = c;
			OnBackground(pos);
			var inner = pos;
			AddPadding(ref inner, GetPadding());
			OnContent(inner);
		}

		private static void AddPadding(ref Rect r, in BoxOffset pad)
		{
			var (pl, pr, pt, pb) = pad;
			var c = r.center;
			r.width -= pl + pr;
			r.height -= pt + pb;
			r.center = c;
		}

		protected virtual void OnInit() { }
		protected virtual float GetHeight(in float w) => 18f;
		protected virtual void OnBackground(in Rect pos) { }
		protected virtual void OnContent(in Rect pos) { }

		protected void DrawText(in Rect pos, GUIContent l, GUIStyle s, in Color c = default)
		{
			var tc = GUI.contentColor;
			GUI.contentColor = _Attribute.TextColor;
			EditorGUI.LabelField(pos, l, s);
			GUI.contentColor = tc;
		}

		private bool _init = false;

	}
}