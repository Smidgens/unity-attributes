// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Display icon for type, similar to System.ComponentModel.DisplayName
	///
	/// Used for example by [InstancedReference]
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
	public sealed class DisplayIconAttribute : __BaseModifier
	{
		public DisplayIconAttribute
		(
			string iconGUID
		)
		{
			this.iconGUID = iconGUID;
		}

		public string iconGUID { get; }
	}
}