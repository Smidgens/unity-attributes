// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
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

		// check if object ref is actually null
		public static bool IsMissing(this UnityObject ob)
		{
			return ((object)ob) != null && !ob;
		}
		
		public static bool IsPrefabInstance(this UnityEngine.Object o)
		{
			return PrefabUtility.GetPrefabInstanceStatus(o) != PrefabInstanceStatus.NotAPrefab;
		}
	}
}

#endif