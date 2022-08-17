// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DropdownColorAttribute))]
	internal class DDColor_ : __ControlDrawer<DropdownColorAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Color | FieldType.String;

		protected override bool HasIcon() => true;

		protected override void OnIcon(in Rect pos, in DrawContext ctx)
		{
			DrawerGUI.ColorPreview(pos, ctx.property.colorValue);
		}

		protected override void OnField(in DrawContext ctx)
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