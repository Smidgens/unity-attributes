// smidgens @ github

#pragma warning disable 0414

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Flags for declaring usability state of fields and buttons
	/// </summary>
	[System.Flags]
	public enum EFieldUsable
	{
		/// <summary>
		/// Never enabled
		/// </summary>
		Never = 0,
		/// <summary>
		/// Enabled in play mode
		/// </summary>
		Play = 1,
		/// <summary>
		/// Enabled outside play mode
		/// </summary>
		Editor = 2,
		/// <summary>
		/// Always enabled
		/// </summary>
		Always = ~0,
	}
}