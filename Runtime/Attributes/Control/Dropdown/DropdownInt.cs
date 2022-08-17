// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class DropdownIntAttribute : __BaseControl
	{
		public int[] Values { get; } = _EMPTY_ARR;
		public DropdownIntAttribute(int start, int n) => Values = GetValues(start, n);
		public DropdownIntAttribute(params int[] values) => Values = values ?? _EMPTY_ARR;

		private static int[] GetValues(int start, int n)
		{
			if (n <= 0) { return _EMPTY_ARR; }
			int[] values = new int[n];
			for (var i = 0; i < n; i++) { values[i] = start + i; }
			return values;
		}

		private static readonly int[] _EMPTY_ARR = new int[0];
	}
}