// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class BoxHeaderAttribute : BaseDecorator
	{
		public string Text { get; } = string.Empty;

		public BoxHeaderAttribute(string text)
		{
			Text = text ?? string.Empty;
		}
	}
}