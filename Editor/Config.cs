// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	/// <summary>
	/// Various constants used across assembly
	/// </summary>
	internal static class Config
	{

		// common labels
		public static class Label
		{
			public const string POPUP_DEFAULT = "<none>";
		}

		public static class Info
		{
			public const string FIELD_NON_NUM = "field is not a number";
			public const string FIELD_NON_FLOAT = "field is non-float";
			public const string FIELD_NON_INT = "field is non-int";
			public const string FIELD_NON_STRING = "field is non-string";
			public const string FIELD_NON_BOOL = "field is non-bool";
			public const string FIELD_NON_COLOR = "field is non-color";
			public const string FIELD_NON_VECTOR2 = "field is non-vector2";
			public const string FIELD_NON_VECTOR3 = "field is non-vector3";
			public const string FIELD_NON_ENUM = "field is non-enum";
			public const string FIELD_NON_COMPONENT = "field is not a component";
			public const string NOT_IMPLEMENTED = "not implemented";
			public const string FIELD_INVALID = "invalid field";
			public const string FIELD_NON_ANIMATOR = "field ref. ≠ Animator";
			public const string FIELD_NON_SKINNEDRENDERER = "field ref. ≠ SkinnedMeshRenderer";
			public const string NO_TYPE_OPTIONS = "No matching types";
			public const string NO_POPUP_OPTIONS = "No options";
		}

		/// <summary>
		/// Paths to resource folders used by plugin
		/// </summary>
		public static class Resource
		{
			public static readonly string BASE_PATH =
			(nameof(Smidgenomics) + "." + nameof(Attributes)).ToLower();

			public static readonly string ROOT = _PREFIX.ToLower();

			public static readonly string
			ICON_SWITCH = ROOT + "{switch}",
			ICON_SEARCH = ROOT + "{search}",
			ICON_FOLDER = ROOT + "{folder}";

			private const string _PREFIX =
			nameof(Smidgenomics) + "." + nameof(Attributes) + "/";
		}
	}
}