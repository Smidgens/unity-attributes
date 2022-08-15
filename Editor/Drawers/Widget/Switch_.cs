// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	[CustomPropertyDrawer(typeof(SwitchAttribute))]
	internal class Switch_ : AttributeDrawer<SwitchAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Bool;

		protected override void DrawField(in FieldContext ctx)
		{
			var prop = ctx.property;
			var cols = ctx.position.CalcColumns(2.0, ctx.position.height * 2f, 1f);
			var label = ctx.attribute.Labels[prop.boolValue.ToInt()];
			if (DrawerGUI.PointerButton(ctx.position))
			{
				prop.boolValue = !prop.boolValue;
			}

			SpriteGUI.AtlasRow(cols[0], _ICON.Value, 2, prop.boolValue.ToInt());

			EditorGUI.LabelField(cols[1], label, EditorStyles.boldLabel);
		}

		// icon atlas
		private readonly Lazy<Texture> _ICON = new Lazy<Texture>(() =>
		{
			return Resources.Load<Texture>(Config.Resource.ICON_SWITCH);
		});

	}
}