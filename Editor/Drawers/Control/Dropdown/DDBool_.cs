// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownBoolAttribute))]
	internal class DDBool_ : __ControlDrawer<DropdownBoolAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Bool;

		protected override void OnField(in DrawContext ctx)
		{
			base.OnField(ctx);
			var prop = ctx.property;
			prop.boolValue = DrawerGUI.BoolDropdown(ctx.position, ctx.property.boolValue, _Attribute.Labels);
		}
	}
}