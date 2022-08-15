// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(TagAttribute))]
	internal class Tag_ : AttributeDrawer<TagAttribute>
	{
		protected override PType GetSupportedTypes() => PType.String;

		protected override void DrawField(in FieldContext ctx)
		{
			Popup.Tag(ctx.position, ctx.property);
		}
	}
}