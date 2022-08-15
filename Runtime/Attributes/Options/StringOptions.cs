// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class StringOptionsAttribute : BaseAttribute
	{
		public bool ShowDefault { get; } = false;
		public string[] Values { get; } = _EMPTY_ARR;
		public StringOptionsAttribute(params string[] values)
		{
			Values = values ?? _EMPTY_ARR;
		}

		public StringOptionsAttribute(bool showDefault, params string[] values)
		{
			ShowDefault = showDefault;
			Values = values ?? _EMPTY_ARR;
		}

		private static readonly string[] _EMPTY_ARR = new string[0];
	}
}