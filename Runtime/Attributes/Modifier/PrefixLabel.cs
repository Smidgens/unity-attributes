// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class FieldNameAttribute : __BaseModifier
	{
		internal string Label { get; } = null;

		public FieldNameAttribute(string label) => Label = label;
	}
}