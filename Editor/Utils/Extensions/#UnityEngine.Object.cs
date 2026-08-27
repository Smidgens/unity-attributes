// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
	using UnityObject = UnityEngine.Object;

	internal static class UnityObject_
	{
		// check if object ref is actually null
		public static bool IsMissing(this UnityObject ob)
		{
			return ((object)ob) != null && !ob;
		}
	}
}

#endif