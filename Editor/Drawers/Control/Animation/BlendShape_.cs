﻿// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BlendShapeAttribute))]
	internal class BlendShape_ : __ControlDrawer<BlendShapeAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String | FieldType.Int;

		protected override void OnField(in FieldContext ctx)
		{
			DrawerGUI.BlendShape(ctx.position, ctx.property, ctx.attribute.RendererField);
		}
	}
}