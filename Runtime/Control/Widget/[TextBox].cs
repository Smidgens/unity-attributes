// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	public sealed class TextBoxAttribute : __BaseControl
	{
		public TextBoxAttribute(int minLines = 2)
		{
			this.minLines = Mathf.Max(1, minLines);
		}
		
		internal int minLines { get; }
		
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
	
	[CustomPropertyDrawer(typeof(TextBoxAttribute))]
	internal sealed class _TextBoxAttribute : __ControlDrawer<TextBoxAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String;

		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
		{
		}

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			_valueLabel.text = prop.stringValue;
			var minHeight = EditorGUIUtility.singleLineHeight * _Attribute.minLines;
			var h = Mathf.Max(DrawerStyles.TextArea.CalcHeight(_valueLabel, Screen.width), minHeight);
			if (label != GUIContent.none && GetCustomLabel() != null)
			{
				h += DrawerStyles.LabelHeightMD;
				h += EditorGUIUtility.standardVerticalSpacing;
			}
			return h;
		}

		private readonly GUIContent _valueLabel = new();

		protected override void OnField(in DrawContext ctx)
		{
			var pos = ctx.position;
			if (ctx.label != GUIContent.none && GetCustomLabel() != null)
			{
				EditorGUI.LabelField(pos.SliceTop(DrawerStyles.LabelHeightMD), ctx.label);
				pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
			}
			ctx.property.stringValue = EditorGUI.TextArea(pos, ctx.property.stringValue, DrawerStyles.TextArea);
		}

	}
}

#endif