// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Popup search for enum
	/// </summary>
	public sealed class SearchEnumAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SearchEnumAttribute))]
	internal sealed class _SearchEnumAttribute : __ControlDrawer<SearchEnumAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Enum;

		protected override void OnField(in DrawContext ctx)
		{
			base.OnField(ctx);
		}
	}
}

#endif