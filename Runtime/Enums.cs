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
		/// Enabled if asset
		/// </summary>
		Asset = 4,
		/// <summary>
		/// Always enabled if outer object exists in scene
		/// </summary>
		AnySceneObject = Play|Editor,
		/// <summary>
		/// Always enabled if outer object is an asset
		/// </summary>
		AnyAsset = Play|Editor|Asset,
		/// <summary>
		/// Always enabled
		/// </summary>
		Always = ~0,
	}
}