// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	// magic editor constants, lt.dan
	internal static class EConstants
	{
		// common labels
		public static class Label
		{
			public const string POPUP_DEFAULT = "<none>";
		}

		public static class Info
		{
			public const string FIELD_NON_INT = "field is non-int";
			public const string FIELD_NON_STRING = "field is non-string";
			public const string NOT_IMPLEMENTED = "not implemented";
			public const string FIELD_INVALID = "invalid field";
			public const string NO_POPUP_OPTIONS = "No options";
		}
	}
}

#endif