// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	internal static class Bool_
	{
		public static int ToInt(this bool v) => v ? 1 : 0;
		public static bool ToBool(this int v) => v == 0 ? false : true;
	}
}