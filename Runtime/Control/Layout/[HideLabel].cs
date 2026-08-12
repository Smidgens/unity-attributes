// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Simply hides prefix label
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("UNITY_EDITOR")]
	public sealed class HideLabelAttribute : __BaseControl
	{
		public bool showArrayIndex;
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(HideLabelAttribute))]
	internal sealed class _HideLabelAttribute : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = attribute as HideLabelAttribute;

			if (attr == null)
			{
				return;
			}

			if (property.propertyPath.EndsWith("]") && attr.showArrayIndex)
			{
				var prefixLabelWidth = EditorStyles.label.CalcSize(_arrLabel).x;
				var labelRect = position;
				labelRect.width = prefixLabelWidth;
				position.width -= prefixLabelWidth;
				position.x += prefixLabelWidth;
				var pLabel = label.text.Substring(7);
				EditorGUI.LabelField(labelRect, pLabel, EditorStyles.miniLabel);
				EditorGUI.PropertyField(position, property, GUIContent.none);
			}
			else
			{
				EditorGUI.PropertyField(position, property, GUIContent.none);
			}
			
		}

		private readonly GUIContent _arrLabel = new ("000");
	}
}

#endif