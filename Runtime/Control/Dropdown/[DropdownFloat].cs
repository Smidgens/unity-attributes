// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	public sealed class DropdownFloatAttribute : __BaseControl
	{
		public float[] Values { get; }

		public DropdownFloatAttribute(params float[] values)
		{
			Values = values ?? Array.Empty<float>();
		}

	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownFloatAttribute))]
	internal sealed class _DropdownFloatAttribute : __ControlDrawer<DropdownFloatAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Float;

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

#endif