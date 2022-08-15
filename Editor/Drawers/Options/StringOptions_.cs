// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(StringOptionsAttribute))]
	internal class StringOptions_ : AttributeDrawer<StringOptionsAttribute>
	{
		protected override PType GetSupportedTypes() => PType.String;

		protected override void DrawField(in FieldContext ctx)
		{
			var a = (StringOptionsAttribute)attribute;

			if (DrawerGUI.PopupButton(ctx.position, ctx.property.stringValue))
			{
				var prop = ctx.property;
				var m = MenuFactory.StringifiedValues(prop.stringValue, a.Values, v =>
				{
					prop.stringValue = v;
					prop.serializedObject.ApplyModifiedProperties();
				});
				m.DropDown(ctx.position);
			}
		}
	}
}