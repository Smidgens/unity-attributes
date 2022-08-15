// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(LayerAttribute))]
	internal class Layer_ : AttributeDrawer<LayerAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Int;

		protected override void DrawField(in FieldContext ctx)
		{
			DrawerGUI.Layer(ctx.position, ctx.property);
		}
	}
}