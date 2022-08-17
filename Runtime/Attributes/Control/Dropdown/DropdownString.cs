// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class DropdownStringAttribute : __BaseControl
	{
		internal string[] StringValues { get; } = {};

		public DropdownStringAttribute(params string[] values)
		{
			StringValues = values ?? _EMPTY_ARR;
		}

		private static readonly string[] _EMPTY_ARR = new string[0];
	}
}