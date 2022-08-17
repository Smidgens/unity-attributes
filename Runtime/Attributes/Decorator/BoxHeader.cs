// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public sealed class BoxHeaderAttribute : __BaseDecorator
	{
		public BoxHeaderAttribute(string text, string textColor = null, string bgColor = null)
		{
			order = -2;
			Text = text ?? string.Empty;
			TextColor = Parse(textColor, TextColor);
			BackgroundColor = Parse(bgColor, BackgroundColor);
		}
	}
}