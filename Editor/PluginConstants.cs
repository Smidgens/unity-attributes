// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;

	// magic editor constants, lt.dan
	internal static class PluginConstants
	{
		// common labels
		public static class Label
		{
			public const string POPUP_UNSET = "(none)";
			public const string POPUP_EMPTY = "No options";
			public const string MISSING = "(missing)";

			public static readonly GUIContent REMOVE_DUPLICATES = new ("Remove duplicates");
			public static readonly GUIContent CLEAR = new ("Clear");
			public static readonly GUIContent CLEAR_MISSING = new ("Remove Missing Items");
			public static readonly GUIContent FIND_CHILD_COMPS = new ("Find Child Components");
		}

		public static class Msg
		{
			public const string FIELD_NON_INT = "field is non-int";
			public const string FIELD_NON_STRING = "field is non-string";
			public const string NOT_IMPLEMENTED = "not implemented";
			public const string FIELD_INVALID = "invalid field";
			public const string NOT_FOUND = "Not found";
			public const string INVALID_TYPE = "Invalid type";
		}
	}
}

#endif