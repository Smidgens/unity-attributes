// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Overrides the default label of field.
	/// (Supplying null hides label)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("UNITY_EDITOR")]
	public sealed class FieldLabelAttribute : __BaseControl
	{
		public FieldLabelAttribute(string label)
		{
			this.label = label;
		}
		internal string label { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(FieldLabelAttribute))]
	internal sealed class _FieldLabelAttribute : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var customLabel = (attribute as FieldLabelAttribute)?.label;
			if (customLabel == null)
			{
				label = GUIContent.none;
			}
			else
			{
				label.text = customLabel;
			}
			EditorGUI.PropertyField(position, property, label);
		}
	}
}

#endif