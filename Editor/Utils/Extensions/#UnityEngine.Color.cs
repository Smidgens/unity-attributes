// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;

	/// <summary>
	/// Extensions for UnityEngine.Color
	/// </summary>
	internal static class Color_
	{
		public static string ToPrettyString(this in Color c)
		{
			return $"({c.r},{c.g},{c.b},{c.a})";
		}

		public static string ToHexString(this in Color c)
		{
			var cs = ColorUtility.ToHtmlStringRGBA(c).ToLower();
			return $"#{cs}";
		}
	}
}

#endif