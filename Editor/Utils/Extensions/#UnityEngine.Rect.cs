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
		public static (Rect,Rect) GetColumns(
			this in Rect r,
			float wl,
			float wr,
			in byte padding
		)
		{
			var remainder = r.width - padding;
			var tratio = 0f;

			if(wl <= 0) { wl = 1f; }
			if(wr <= 0) { wr = 1f; }

			Rect rl = r, rr = r;

			if(wl > 1f)
			{
				rl.width = wl;
				remainder -= wl;
			}
			else { tratio += wl; }

			if(wr > 1f)
			{
				rr.width = wr;
				remainder -= wr;
			}
			else { tratio += wr; }

			if (wl <= 1f) { tratio = wl; }
			if(wr < 1f) { tratio = wr; }

			if (tratio > 0f && wl <= 1f)
			{
				wl = wl / tratio;
				rl.width = remainder * wl;
			}
			if (tratio > 0f && wr <= 1f)
			{
				wr = wr / tratio;
				rr.width = remainder * wr;
			}
			rr.position += new Vector2(rl.width + padding, 0f);
			return (rl,rr);
		}


		public static Rect Resize(this Rect r, in float s)
		{
			return Resize(r, s, _PIVOT_CENTER);
		}

		public static Rect ResizeW(this Rect r, in float s)
		{
			// todo: use pivot
			var c = r.center;
			r.width += s;
			r.center = c;
			return r;
		}

		public static Rect Resize(this Rect r, in float s, in Vector2 pivot)
		{
			// todo: use pivot
			var c = r.center;
			r.width += s;
			r.height += s;
			r.center = c;
			return r;
		}

		public static Rect[] CalcRows(this in Rect pos, params float[] widths)
		{
			return CalcRows(pos, 0.0, widths);
		}

		public static Rect[] CalcRows(this in Rect pos, in double pad, params float[] sizes)
		{
			var r = new Rect[sizes.Length];
			if (sizes.Length == 0) { return r; }
			var (poffset, ptotal) = GetSplitPadding(sizes.Length, pos.height, pad);
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

		public static Rect[] CalcColumns(this Rect pos, double pad, params float[] widths)
		{
			var r = new Rect[widths.Length];
			if (widths.Length == 0) { return r; }
			var (poffset, ptotal) = GetSplitPadding(widths.Length, pos.width, pad);
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

		private static readonly Vector2 _PIVOT_CENTER = Vector2.one * 0.5f;

		private static (float, float) GetSplitPadding(int n, float v, double p)
		{
			if (n < 2) { return default; }
			var o = System.Convert.ToSingle(p);
			// ratio
			if (o < 1) { o = o * v; }
			return (o, o * (n - 1));
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

		public static Rect SliceSquare(this ref Rect r, bool end = false)
		{
			if (r.width < r.height)
			{
				return !end ? r.SliceTop(r.width) : r.SliceBottom(r.width);
			}
			return !end ? r.SliceLeft(r.width) : r.SliceRight(r.width);
		}

	
	}
}

#endif