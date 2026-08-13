// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Set size of specific inlined field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class InlineWidthAttribute : __BaseModifier
	{
		public InlineWidthAttribute(float w)
		{
			width = Mathf.Max(w, 0f);
		}

		/// <summary>
		/// Specify width of inner field
		/// </summary>
		public InlineWidthAttribute(string field, float w) : this(w)
		{
			this.fieldName = field;
		}
		internal float width { get; }
		internal string fieldName { get; }
	}
}