// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	/// <summary>
	/// 0-1 float slider
	/// </summary>
	public class Slider01Attribute : __BaseControl
	{
		public const int DEFAULT_PRECISION = -1;
		public const float DEFAULT_STEP = -1f;

		/// <summary>
		/// Decimal rounding
		/// </summary>
		public int Precision { get; } = DEFAULT_PRECISION;

		public float Step { get; } = DEFAULT_STEP;

		/// <summary>
		/// Default settings
		/// </summary>
		public Slider01Attribute(){}

		/// <summary>
		/// Init with precision
		/// </summary>
		public Slider01Attribute(int precision)
		{
			Precision = ClampMin(precision, DEFAULT_PRECISION);
		}

		/// <summary>
		/// Init with step + precision
		/// </summary>
		public Slider01Attribute(float step, int precision = DEFAULT_PRECISION)
		{
			Precision = ClampMin(precision, DEFAULT_PRECISION);
			Step = Mathf.Clamp01(step);
		}

		private static int ClampMin(int v, in int min)
		{
			if (v < min) { v = min; }
			return v;
		}

	}
}