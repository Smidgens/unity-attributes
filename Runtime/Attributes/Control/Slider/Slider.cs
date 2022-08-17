// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Number slider with rounding support
	/// </summary>
	public class SliderAttribute : __BaseControl
	{
		internal const int DEFAULT_PRECISION = -1;
		internal const float DEFAULT_STEP = 0f;

		/// <summary>
		/// Decimal rounding
		/// </summary>
		internal int Precision { get; } = DEFAULT_PRECISION;
		internal float Min { get; } = default;
		internal float Max { get; } = default;
		internal float Step { get; } = DEFAULT_STEP;

		/// <summary>
		/// Default settings
		/// </summary>
		public SliderAttribute(float min, float max)
		: this(min, max, DEFAULT_PRECISION) { }

		public SliderAttribute(int min, int max, int step = -1)
			: this((float)min,max,step) { }
		
		public SliderAttribute(double min, double max, int step = -1)
			: this((float)min, (float)max, (float)step) { }


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