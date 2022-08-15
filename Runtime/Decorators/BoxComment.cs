// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	public class BoxCommentAttribute : BaseDecorator
	{
		public string Text { get; } = string.Empty;
		public sbyte Type { get; } = 0;

		public BoxCommentAttribute(string text, sbyte type = 0)
		{
			order = 1;
			Text = text ?? string.Empty;
			Type = (sbyte)Mathf.Clamp(type, 0, 3);
		}
	}
}