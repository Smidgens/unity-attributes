// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class PrefixLabelAttribute : __BaseModifier
	{
		internal string Label { get; } = null;

		public PrefixLabelAttribute(string label) => Label = label;
	}
}