﻿// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(TagAttribute))]
	internal class Tag_ : __ControlDrawer<TagAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String;

		protected override void OnField(in FieldContext ctx)
		{
			Popup.Tag(ctx.position, ctx.property);
		}
	}
}