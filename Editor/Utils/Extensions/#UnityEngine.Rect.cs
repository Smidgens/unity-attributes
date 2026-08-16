// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;

	/// <summary>
	/// Extensions for UnityEngine.Rect
	/// </summary>
	internal static class Rect_
	{
		public static void Resize(this ref Rect r, in float s) => r.Resize(s, s, s, s);

		public static void Resize(this ref Rect r, float lr, in float tb) => r.Resize(lr, lr, tb, tb);
		
		public static void Resize
		(
			this ref Rect rect,
			in float l,
			in float r,
			in float t,
			in float b
		)
		{
			var c = rect.center;
			rect.width += l + r;
			rect.height += t + b;
			rect.center = c;
		}
		
		public static Rect Resized(this Rect r, in float s)
		{
			var newRect = r;
			newRect.Resize(s);
			return newRect;
		}

		public static Rect Padded(this Rect r, RectOffset ro)
		{
			var center = r.center;
			r.height -= ro.bottom + ro.top;
			r.width -= ro.left + ro.right;
			r.center = center;
			return r;
		}

		public static Rect SliceBottom(this ref Rect r, in float h)
		{
			var r2 = r;
			r2.height = h;
			r.height -= h;
			r2.y += r.height;
			return r2;
		}
		
		public static Rect SliceLeft(this ref Rect r, in float w)
		{
			var r2 = r;
			r2.width = w;
			r.width -= w;
			r.x += w;
			return r2;
		}
		
		public static Rect SliceRight(this ref Rect r, in float w)
		{
			var r2 = r;
			r2.width = w;
			r.width -= w;
			r2.x += r.width;
			return r2;
		}
		
		public static Rect SliceTop(this ref Rect r, in float h)
		{
			var r2 = r;
			r2.height = h;
			r.height -= h;
			r.position += new Vector2(0f, h);
			return r2;
		}

	
	}
}

#endif