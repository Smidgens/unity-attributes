// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Number slider with rounding support
	/// </summary>
	public sealed class SliderAttribute : __BaseControl
	{
		internal const int DEFAULT_PRECISION = -1;
		internal const float DEFAULT_STEP = 0f;

		/// <summary>
		/// Decimal rounding
		/// </summary>
		internal int Precision { get; } = DEFAULT_PRECISION;
		internal float Min { get; }
		internal float Max { get; }
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
			(a, b) = (b, a);
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SliderAttribute))]
	internal sealed class _SliderAttribute : __ControlDrawer<SliderAttribute>
	{
		protected override void OnField(in DrawContext ctx)
		{
			var a = _Attribute;
			DrawerGUI.Slider(ctx.position, ctx.property, a.Min, a.Max, a.Step, a.Precision);
		}
	}
}

#endif