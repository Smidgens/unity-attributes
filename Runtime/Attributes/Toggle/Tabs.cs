// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draw bool fields as tabs
	/// </summary>
	public class TabsAttribute : BaseAttribute
	{
		public int Rows { get; } = 1;
		public string[] Fields { get; } = { };

		public bool StretchHeight { get; set; } = false;

		public TabsAttribute(params string[] toggleFields)
		{
			if (toggleFields == null) { return; }
			Fields = toggleFields;
		}

		public TabsAttribute(int rows, params string[] toggleFields)
		{
			if (rows > 0) { Rows = rows; }
			Fields = toggleFields ?? new string[0];
		}
	}
}
