// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	public sealed class DropdownTypeAttribute : __BaseControl
	{
		public DropdownTypeAttribute(params Type[] types)
		{
			Values = types;
			// todo: labels
		}

		internal string[] Labels { get; } = { };
		internal Type[] Values { get; } = { };
	}
}