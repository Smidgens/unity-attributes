// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;

	/// <summary>
	/// Extensions for UnityEngine.Rect
	/// </summary>
	internal static class Rect_
	{
		/// <summary>
		/// Split rect into vertical subrects
		/// </summary>
		public static Rect[] SplitVertically(this in Rect r, float padding, params float[] heights)
		{
			float absoluteHeight = padding * (heights.Length - 1);
			for (int i = 0; i < heights.Length; i++)
			{
				if (heights[i] <= 1f) { continue; }
				absoluteHeight += heights[i];
			}
			float remainder = r.height - absoluteHeight;
			if (remainder < 0) { remainder = 0f; }

			Rect[] rects = new Rect[heights.Length];
			float offset = 0f;
			for (int i = 0; i < heights.Length; i++)
			{
				rects[i] = new Rect(r.x, r.y + offset, r.width, heights[i] <= 1f ? heights[i] * remainder : heights[i]);
				offset += rects[i].height + padding;
			}
			return rects;
		}

		public static Rect[] SplitHorizontally(this Rect r, int n, double pad = 0.0)
		{
			return r.SplitHorizontally(pad, r.width.Split(n));
		}

		public static Rect[] SplitHorizontally(this Rect pos, params float[] widths)
		{
			return SplitHorizontally(pos, 0.0, widths);
		}

		public static Rect[] SplitHorizontally(this Rect pos, double pad, params float[] widths)
		{
			var r = new Rect[widths.Length];
			if (widths.Length == 0) { return r; }
			var (poffset, ptotal) = GetSplitPadding2(widths.Length, pos.width, pad);
			var totalSize = pos.width - ptotal;
			var w = totalSize.Split(widths);
			var offset = 0f;
			for (var i = 0; i < w.Length; i++)
			{
				r[i] = pos;
				r[i].x += offset;
				r[i].width = w[i];
				offset += w[i] + poffset;
			}
			return r;
		}


		private static (float, float) GetSplitPadding2(int n, float v, double p)
		{
			if (n < 2) { return default; }
			var o = System.Convert.ToSingle(p);
			// ratio
			if (o < 1) { o = o * v; }
			return (o, o * (n - 1));
		}

		public static Rect Pad(this Rect r, float v)
		{
			var c = r.center;
			r.size -= Vector2.one * v;
			r.center = c;
			return r;
		}

		public static Rect PadLeft(this Rect r, float v)
		{
			var c = r.center;
			r.width -= v;
			r.position += Vector2.right * v;
			return r;
		}

		private static SplitPad GetSplitPadding(int n, float v, double p)
		{
			if (n < 2) { return default; }
			var o = System.Convert.ToSingle(p);
			// ratio
			if (o < 1) { o = o * v; }
			return new SplitPad { offset = o, total = o * (n - 1) };
		}

		public static Rect Resize(this Rect r, in float s)
		{
			var c = r.center;
			r.width += s;
			r.height += s;
			r.center = c;
			return r;
		}

		public static Rect[] SubdivideX(this in Rect r, in int n, in double pad = 0.0)
		{
			return r.SubdivideX(pad, r.width.Subdivide(n));
		}

		public static Rect[] SubdivideX(this in Rect pos, params float[] widths)
		{
			return SubdivideX(pos, 0.0, widths);
		}

		public static Rect[] SubdivideX(this in Rect pos, in double pad, params float[] widths)
		{
			if(widths.Length < 2)
			{
				return new Rect[] { pos };
			}

			var r = new Rect[widths.Length];

			var (poffset,ptotal) = GetDividePadding(widths.Length, pos.width, pad);

			var totalSize = pos.width - ptotal;
			var w = totalSize.Subdivide(widths);

			var offset = 0f;
			for (var i = 0; i < w.Length; i++)
			{
				r[i] = pos;
				r[i].x += offset;
				r[i].width = w[i];
				offset += w[i] + poffset;
			}
			return r;
		}

		public static Rect[] SubdivideY(this in Rect r, in int n, in double pad = 0.0)
		{
			return r.SubdivideY(pad, r.height.Subdivide(n));
		}

		public static Rect[] SubdivideY(this in Rect pos, params float[] widths)
		{
			return SubdivideY(pos, 0.0, widths);
		}

		public static Rect[] SubdivideY(this in Rect pos, in double pad, params float[] sizes)
		{
			var r = new Rect[sizes.Length];
			if (sizes.Length == 0) { return r; }
			var (poffset,ptotal) = GetDividePadding(sizes.Length, pos.height, pad);
			var totalSize = pos.height - ptotal;

			var weights = totalSize.Subdivide(sizes);

		
			var offset = 0f;
			for (var i = 0; i < weights.Length; i++)
			{
				r[i] = pos;
				r[i].y += offset;
				r[i].height = weights[i];
				offset += weights[i] + poffset;
			}
			return r;
		}

		private static (float,float) GetDividePadding(in int n, in float v, in double p)
		{
			if (n < 2) { return default; }
			var o = System.Convert.ToSingle(p);
			// ratio
			if (o < 1) { o = o * v; }
			return (o, o * (n - 1));
		}

		private struct SplitPad { public float offset, total; }
	}
}