// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	public sealed class DropdownColorAttribute : __BaseControl
	{
		public static readonly Color DEFAULT_COLOR = Color.clear;
		internal readonly string[] Labels = Array.Empty<string>();
		internal readonly Color[] Values = Array.Empty<Color>();
		internal readonly string[] HTMLValues = Array.Empty<string>();

		public string GetLabel(in int i)
		{
			return i >= 0 && i < Labels.Length ? Labels[i] : "?";
		}

		public DropdownColorAttribute(params string[] htmlColors)
		{
			if (htmlColors == null || htmlColors.Length == 0) { return; }
			var values = new Color[htmlColors.Length];
			Labels = htmlColors;

			for (var i = 0; i < values.Length; i++)
			{
				if (ColorUtility.TryParseHtmlString(htmlColors[i], out Color c))
				{
					values[i] = c;
				}
				else { values[i] = DEFAULT_COLOR; }
			}
			Values = values;
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DropdownColorAttribute))]
	internal sealed class _DropdownColorAttribute : __ControlDrawer<DropdownColorAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Color | EFieldType.String;

		protected override bool HasIcon() => true;

		protected override void OnIcon(in Rect pos, in DrawContext ctx)
		{
			DrawColorPreview(pos, ctx.property.colorValue);
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
		
		private static void DrawColorPreview(in Rect pos, in Color c)
		{
			EditorGUI.DrawRect(pos, c);
		}
	}
}

#endif