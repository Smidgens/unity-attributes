// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Hex color
	/// </summary>
	public sealed class HexColorAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
	
	[CustomPropertyDrawer(typeof(HexColorAttribute))]
	internal sealed class _HexColorAttribute : __ControlDrawer<HexColorAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			HexColor(ctx.position, ctx.property);
		}

		private static void HexColor(in Rect pos, SerializedProperty prop)
		{
			// valid type?
			if (prop.propertyType != SerializedPropertyType.String)
			{
				DrawerGUI.MutedInfo(pos, PluginConstants.Msg.FIELD_NON_STRING);
				return;
			}

			EditorGUI.BeginChangeCheck();

			var newColor = EditorGUI.ColorField(pos, HexToColor(prop.stringValue));
			if (EditorGUI.EndChangeCheck())
			{
				prop.stringValue = newColor.ToHexString();
			}

			if (!GUI.enabled)
			{
				EditorGUI.DrawRect(pos, Color.black * 0.4f);
			}

		}

		private static Color HexToColor(in string hex)
		{
			if (ColorUtility.TryParseHtmlString(hex, out var c))
			{
				return c;
			}
			return Color.clear;
		}
	}
}

#endif