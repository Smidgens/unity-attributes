﻿// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownIntAttribute))]
	internal class DDInt_ : __ControlDrawer<DropdownIntAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			DrawerGUI.IntegerDropdown(ctx.position, ctx.property, _Attribute.Values);
		}
	}
}