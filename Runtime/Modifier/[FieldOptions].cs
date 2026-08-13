// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Options for field drawer
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldOptionsAttribute : __BaseModifier
	{
		public FieldOptionsAttribute
		(
			string label = "",
			byte indent = 0
		)
		{
			this.label = label;
			this.indent = indent;
		}

		internal byte indent { get; }
		internal string label { get; }
	}
}