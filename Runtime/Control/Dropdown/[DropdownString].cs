// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	public sealed class DropdownStringAttribute : __BaseControl
	{
		internal string[] StringValues { get; }

		public DropdownStringAttribute(params string[] values)
		{
			StringValues = values ?? Array.Empty<string>();
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownStringAttribute))]
	internal sealed class _DropdownStringAttribute : __ControlDrawer<DropdownStringAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String;

		protected override void OnField(in DrawContext ctx)
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

#endif