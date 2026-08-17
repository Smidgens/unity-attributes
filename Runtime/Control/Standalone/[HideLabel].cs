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
			EditorGUI.PropertyField(position, property, GUIContent.none);
		}
	}
}

#endif