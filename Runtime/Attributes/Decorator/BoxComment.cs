// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class BoxCommentAttribute : __BaseDecorator
	{
		public BoxCommentAttribute(
			string text,
			string textColor = null,
			string bgColor = null
		)
		{
			order = -1;
			Text = text ?? string.Empty;
			TextColor = Parse(textColor, TextColor);
			BackgroundColor = Parse(bgColor, BackgroundColor);
		}
	}
}