// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	/// <summary>
	/// 0-1 float slider with precision and step options
	/// </summary>
	public sealed class Slider01Attribute : __BaseControl
	{
		internal int precision { get; }
		internal float step { get; }

		public Slider01Attribute(float step = 0, int precision = 1)
		{
			this.precision = Mathf.Max(precision, 1);
			this.step = Mathf.Max(step, 0f);
		}
	}
}