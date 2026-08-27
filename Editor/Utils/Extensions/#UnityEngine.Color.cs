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
		public static Color Fade(this Color c, float a)
		{
			c.a = a;
			return c;
		}

		public static string ToHexString(this in Color c)
		{
			var cs = ColorUtility.ToHtmlStringRGBA(c).ToLower();
			return $"#{cs}";
		}
	}
}

#endif