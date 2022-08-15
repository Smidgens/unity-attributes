// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class BoxHeaderAttribute : BaseDecorator
	{
		public BoxHeaderAttribute(string t) => Text = t ?? string.Empty;
		internal string Text { get; } = string.Empty;
	}
}