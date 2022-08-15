// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityObject = UnityEngine.Object;

	internal static class UnityObject_
	{
		/// <summary>
		/// Finds GUID for object if valid asset
		/// </summary>
		public static string GetAssetGUID(this UnityObject a)
		{
			if (!a) { return null; }
			var path = AssetDatabase.GetAssetPath(a);
			return !string.IsNullOrEmpty(path) ? AssetDatabase.AssetPathToGUID(path) : null;
		}
	}
}