// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Hex color
	/// </summary>
	public sealed class HexColorAttribute : __BaseControl
	{
		public HexColorAttribute
		(
			bool showAlpha = true,
			bool hdr = false
		)
		{
			this.showAlpha = showAlpha;
			this.hdr = hdr;
		}
		
		internal bool showAlpha { get; }
		internal bool hdr { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Reflection;
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

		private void HexColor(in Rect pos, SerializedProperty prop)
		{
			// valid type?
			if (prop.propertyType != SerializedPropertyType.String)
			{
				DrawerGUI.MutedInfo(pos, PluginConstants.Msg.FIELD_NON_STRING);
				return;
			}

			EditorGUI.BeginChangeCheck();

			var alpha = _Attribute.showAlpha;
			var hdr = _Attribute.hdr;

			var newColor = EditorGUI.ColorField(pos, GUIContent.none, HexToColor(prop.stringValue), true, alpha, hdr); 
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