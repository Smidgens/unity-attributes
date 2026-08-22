// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	internal static class Enums_
	{
		// syntactic sugar for getting use state based on play mode state
		public static bool GetUseState(this EFieldUsable flags, Object serializedTarget = null)
		{
			var result = false;
			
			if (Application.isPlaying && flags.HasFlag(EFieldUsable.Play))
			{
				result = true;
			}
			else if (!Application.isPlaying && flags.HasFlag(EFieldUsable.Editor))
			{
				result = true;
			}

			if (result && serializedTarget && !flags.HasFlag(EFieldUsable.Asset))
			{
				if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(serializedTarget)))
				{
					result = false;
				}
			}
		
			return result;
		}
	}
}

#endif