// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownIntAttribute))]
	internal class IntOptions_ : __ControlDrawer<DropdownIntAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Int;

		protected override void OnField(in FieldContext ctx)
		{
			DrawerGUI.IntegerDropdown(ctx.position, ctx.property, ctx.attribute.Values);
		}
	}
}