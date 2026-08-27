// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	internal static class Float_
	{
		public static float Round(this in float v, int precision = 1)
		{
			if (precision < 1) { precision = 1; }
			return (float)System.Math.Round((decimal)v, precision);
		}
	}
}

#endif