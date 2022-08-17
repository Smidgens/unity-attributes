// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownStringAttribute))]
	internal class StringOptions_ : __ControlDrawer<DropdownStringAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String;

		protected override void OnField(in FieldContext ctx)
		{
			var a = (DropdownStringAttribute)attribute;

			if (DrawerGUI.PopupButton(ctx.position, ctx.property.stringValue))
			{
				var prop = ctx.property;
				var m = MenuFactory.StringifiedValues(prop.stringValue, a.StringValues, v =>
				{
					prop.stringValue = v;
					prop.serializedObject.ApplyModifiedProperties();
				});
				m.DropDown(ctx.position);
			}
		}
	}
}