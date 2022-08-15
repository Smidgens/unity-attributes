// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(FilterEnumAttribute))]
	internal class FilterEnum_ : AttributeDrawer<FilterEnumAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Enum;

		protected override void DrawField(in FieldContext ctx)
		{
			base.DrawField(ctx);
		}
	}
}