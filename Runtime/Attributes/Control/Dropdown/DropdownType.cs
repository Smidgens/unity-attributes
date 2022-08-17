// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Dropdown of Type values (saved as string)
	/// </summary>
	public sealed class DropdownTypeAttribute : __BaseControl
	{
		public DropdownTypeAttribute(params Type[] types)
		{
			Types = types;
		}
		internal readonly Type[] Types = null;
	}
}