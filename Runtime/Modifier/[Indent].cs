// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class IndentAttribute : __BaseModifier
	{
		internal byte Indent { get; }

		public IndentAttribute(byte indent = 1)
		{
			Indent = indent;
		}
	}
}