// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;

	/// <summary>
	/// Extensions for UnityEngine.GUIContent
	/// </summary>
	internal static class GUIContent_
	{
		public static bool IsEmpty(this GUIContent l) => l == GUIContent.none;
	}
}