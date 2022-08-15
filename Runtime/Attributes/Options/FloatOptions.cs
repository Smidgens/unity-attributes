// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class FloatOptionsAttribute : BaseAttribute
	{
		public float[] Values { get; } = _EMPTY_ARR;
		public FloatOptionsAttribute(params float[] values) => Values = values ?? _EMPTY_ARR;

		private static readonly float[] _EMPTY_ARR = new float[0];
	}
}