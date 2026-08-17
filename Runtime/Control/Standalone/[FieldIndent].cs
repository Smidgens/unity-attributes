// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Adds extra indent to field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("UNITY_EDITOR")]
	public sealed class FieldIndentAttribute : __BaseControl
	{
		public FieldIndentAttribute(byte indent)
		{
			this.indent = indent;
		}
		internal byte indent { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(FieldIndentAttribute))]
	internal sealed class _FieldIndentAttribute : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = (attribute as FieldIndentAttribute)!;
			EditorGUI.indentLevel += attr.indent;
			EditorGUI.PropertyField(position, property, label);
			EditorGUI.indentLevel -= attr.indent;
		}
	}
}

#endif