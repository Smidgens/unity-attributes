// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Number slider with rounding support
	/// </summary>
	public class SliderAttribute : BaseAttribute
	{
		public const int DEFAULT_PRECISION = -1;
		public const float DEFAULT_STEP = 0f;
		/// <summary>
		/// Decimal rounding
		/// </summary>
		public int Precision { get; } = DEFAULT_PRECISION;
		public float Min { get; } = default;
		public float Max { get; } = default;
		public float Step { get; } = DEFAULT_STEP;

		/// <summary>
		/// Default settings
		/// </summary>
		public SliderAttribute(float min, float max)
		: this(min, max, DEFAULT_PRECISION) { }

		/// <summary>
		/// Init slider with rounding
		/// </summary>
		public SliderAttribute(float min, float max, int precision)
		{
			if (min > max) { Swap(ref min, ref max); }
			Min = min;
			Max = max;
			Precision = precision;
		}

		public SliderAttribute(float min, float max, float step)
		{
			if (min > max) { Swap(ref min, ref max); }
			Min = min;
			Max = max;
			Step = step;
		}

		private static void Swap<T>(ref T a, ref T b)
		{
			var t = a;
			a = b;
			b = t;
		}
	}
}