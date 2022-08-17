// smidgens @ github

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

		public static Rect[] CalcColumns(this Rect r, int n, double pad = 0.0)
		{
			return r.CalcColumns(pad, r.width.Split(n));
		}

		public static Rect[] CalcColumns(this Rect pos, params float[] widths)
		{
			return CalcColumns(pos, 0.0, widths);
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

		private static readonly Vector2
		_PIVOT_CENTER = Vector2.one * 0.5f;

		private static (float, float) GetSplitPadding(int n, float v, double p)
		{
			if (n < 2) { return default; }
			var o = System.Convert.ToSingle(p);
			// ratio
			if (o < 1) { o = o * v; }
			return (o, o * (n - 1));
		}



		private struct SplitPad { public float offset, total; }

		public static Rect[] SplitVertically(this Rect r, int n, double pad = 0.0)
		{
			return r.SplitVertically(pad, r.height.Split(n));
		}

		public static Rect[] SplitVertically(this Rect pos, params float[] widths)
		{
			return SplitVertically(pos, 0.0, widths);
		}

		public static Rect[] SplitVertically(this Rect pos, double pad, params float[] sizes)
		{
			var r = new Rect[sizes.Length];
			if (sizes.Length == 0) { return r; }
			var (soffset,stotal) = GetSplitPadding(sizes.Length, pos.height, pad);
			var totalSize = pos.height - stotal;

			var weights = totalSize.Split(sizes);

			var offset = 0f;
			for (var i = 0; i < weights.Length; i++)
			{
				r[i] = pos;
				r[i].y += offset;
				r[i].height = weights[i];
				offset += weights[i] + soffset;
			}
			return r;
		}


	
	}
}