// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BoolOptionsAttribute))]
	internal class BoolOptions_ : AttributeDrawer<BoolOptionsAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Bool;

		protected override void DrawField(in FieldContext ctx)
		{
			base.DrawField(ctx);
			var prop = ctx.property;
			prop.boolValue = DrawerGUI.BoolDropdown(ctx.position, ctx.property.boolValue, ctx.attribute.Labels);
		}
	}
}