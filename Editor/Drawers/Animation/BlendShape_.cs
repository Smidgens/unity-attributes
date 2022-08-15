// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BlendShapeAttribute))]
	internal class BlendShape_ : AttributeDrawer<BlendShapeAttribute>
	{
		protected override PType GetSupportedTypes() => PType.String | PType.Int;

		protected override void DrawField(in FieldContext ctx)
		{
			DrawerGUI.BlendShape(ctx.position, ctx.property, ctx.attribute.RendererField);
		}
	}
}