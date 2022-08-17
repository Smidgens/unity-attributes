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
			base.Text = text ?? string.Empty;
			base.TextColor = Parse(textColor, base.TextColor);
			BackgroundColor = Parse(bgColor, BackgroundColor);
		}
	}
}