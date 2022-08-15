// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ColorOptionsAttribute))]
	internal class ColorOptions_ : AttributeDrawer<ColorOptionsAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Color | PType.String;

		protected override bool HasIcon() => true;

		protected override void DrawIcon(in Rect pos, in FieldContext ctx)
		{
			DrawerGUI.ColorPreview(pos, ctx.property.colorValue);
		}

		protected override void DrawField(in FieldContext ctx)
		{
			var blabel = ctx.property.colorValue.ToPrettyString();

			if (DrawerGUI.PopupButton(ctx.position, blabel))
			{
				var prop = ctx.property;
				var m = MenuFactory.StringifiedValues(
					ctx.property.colorValue,
					_Attribute.Values,
					v =>
					{
						prop.colorValue = v;
						prop.serializedObject.ApplyModifiedProperties();
					},
					(c, i) => _Attribute.GetLabel(i)
				);
				m.DropDown(ctx.position);
			}
		}
	}
}