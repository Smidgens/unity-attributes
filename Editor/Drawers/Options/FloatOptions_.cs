// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(FloatOptionsAttribute))]
	internal class FloatOptions_ : AttributeDrawer<FloatOptionsAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Float;

		protected override void DrawField(in FieldContext ctx)
		{
			// popup
			var a = (FloatOptionsAttribute)attribute;

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