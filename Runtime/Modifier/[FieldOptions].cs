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
			byte indent = 0,
			EFieldUsable useFlags = EFieldUsable.Always
		)
		{
			this.label = label;
			this.indent = indent;
			this.useFlags = useFlags;
		}

		internal EFieldUsable useFlags { get; }
		internal byte indent { get; }
		internal string label { get; }
	}
}