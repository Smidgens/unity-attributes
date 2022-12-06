// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Linq;

	internal static class Float_
	{
		public static float Round(this in float v, int precision = 1)
		{
			if (precision < 1) { precision = 1; }
			return (float)System.Math.Round((decimal)v, precision);
		}

		public static float[] Split(this float v, int n)
		{
			if (n < 1) { return new float[] { v }; }
			var values = new float[n];
			var w = 1f / n;
			for (var i = 0; i < values.Length; i++) { values[i] = w; }
			return values;
		}

		public static float[] Split(this float v, params float[] weights)
		{
			if (weights.Length == 0) { return new float[0]; }
			var flex = v;
			// absolute weights, >1
			foreach (var w in weights) { if (w > 1f) { flex -= w; } }
			return weights.Select((w, i) => w > 1f ? w : w * flex).ToArray();
		}

		public static float[] Subdivide(this float v, in int n)
		{
			if (n < 1) { return new float[] { v }; }
			float[] values = new float[n];
			float w = 1f / n;
			for (int i = 0; i < n; i++) { values[i] = w; }
			return values;
		}


		public static float[] Subdivide(this float v, params float[] sizes)
		{
			if (sizes.Length == 0) { return Empty.Array.FLOAT; }
			float flex = v; 
			// sum up remaining size after
			for(int i = 0; i < sizes[i]; i++)
			{
				if(sizes[i] > 1f) { flex -= sizes[i]; }
			}
			// compute proportional sizes
			for(int i = 0; i < sizes.Length; i++)
			{
				if (sizes[i] <= 1f) { sizes[i] = sizes[i] * flex; }
			}
			return sizes;
		}

	}
}

#endif