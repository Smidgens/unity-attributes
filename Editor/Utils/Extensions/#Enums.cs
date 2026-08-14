// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;

	internal static class Enums_
	{
		// syntactic sugar for getting use state based on play mode state
		public static bool GetUseState(this EFieldUsable flags)
		{
			if (Application.isPlaying && flags.HasFlag(EFieldUsable.Play))
			{
				return true;
			}

			if (!Application.isPlaying && flags.HasFlag(EFieldUsable.Editor))
			{
				return true;
			}
			return false;
		}
	}
}

#endif