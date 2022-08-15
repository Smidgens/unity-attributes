// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class BoolOptionsAttribute : BaseAttribute
	{
		public static readonly string[] DEFAULT_LABELS = { "False", "True" };
		public string[] Labels { get; } = DEFAULT_LABELS;
		public BoolOptionsAttribute() { }

		public BoolOptionsAttribute(string l0, string l1)
		{
			Labels = new string[] { l0 ?? DEFAULT_LABELS[0], l1 ?? DEFAULT_LABELS[1] };
		}
	}
}