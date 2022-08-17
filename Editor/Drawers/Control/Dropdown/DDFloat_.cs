// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownFloatAttribute))]
	internal class DDFloat_ : __ControlDrawer<DropdownFloatAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Float;

		protected override void OnField(in DrawContext ctx)
		{
			// popup
			var a = (DropdownFloatAttribute)attribute;

			if (DrawerGUI.PopupButton(ctx.position, ctx.property.floatValue.ToString()))
			{
				var prop = ctx.property;
				var m = MenuFactory.StringifiedValues(prop.floatValue, a.Values, v =>
				{
					prop.floatValue = v;
					prop.serializedObject.ApplyModifiedProperties();
				});
				m.DropDown(ctx.position);
			}
		}
	}
}