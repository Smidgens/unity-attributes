// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class BoxLinkAttribute : __BaseDecorator
	{
		public BoxLinkAttribute(string text, string url)
		{
			order = -0;
			URL = url;
			base.Text = text;
		}
		internal string URL { get; } = "";
	}
}