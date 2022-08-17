// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Set size of specific inlined field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class FieldSizeAttribute : __BaseModifier
	{
		public FieldSizeAttribute(string name, float size)
		{
			Name = name;
			Size = size;
		}
		internal string Name { get; }
		internal float Size { get; } = -1f;
	}
}