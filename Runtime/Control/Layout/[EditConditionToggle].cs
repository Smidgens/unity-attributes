// smidgens @ github

// ReSharper disable PossibleNullReferenceException
// ReSharper disable ReplaceSubstringWithRangeIndexer

namespace Smidgenomics.Unity.Attributes
{
	public sealed class EditConditionToggleAttribute : __BaseControl
	{
		internal string toggleField { get; }
		internal bool hide { get; }

		public EditConditionToggleAttribute(string toggleField, bool hide = true)
		{
			this.toggleField = toggleField;
			this.hide = hide;
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;
	
	[CustomPropertyDrawer(typeof(EditConditionToggleAttribute))]
	internal sealed class _EditConditionToggleAttribute : __ControlDrawer<EditConditionToggleAttribute>
	{
		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			if (prop == null)
			{
				return 0;
			}

			var toggleState = GetToggleState(prop);

			if (!toggleState && _Attribute.hide)
			{
				return 0f;
			}
			return EditorGUI.GetPropertyHeight(prop);
		}

		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
		{
			var toggleState = GetToggleState(prop);
			
			if (!toggleState && _Attribute.hide)
			{
				return;
			}
			
			DrawerGUI.IndentRect(ref pos, _ExtraIndent);
	
			base.OnLabel(ref pos, prop, l);
		}

		protected override void OnField(in DrawContext ctx)
		{
			var pos = ctx.position;
			var prop = ctx.property;
			var l = ctx.label;
			
			if (pos.height == 0)
			{
				return;
			}

			var toggleState = GetToggleState(prop);

			if (!toggleState && _Attribute.hide)
			{
				return;
			}

			var tEnabled = GUI.enabled;
			GUI.enabled = toggleState;

			EditorGUI.PropertyField(pos, prop, GUIContent.none);

			GUI.enabled = tEnabled;
		}

		private bool GetToggleState(SerializedProperty currentProp)
		{
			var fieldName = currentProp.name;
			var basePath = currentProp.propertyPath.Substring(0, currentProp.propertyPath.Length - fieldName.Length);
			var togglePath = $"{basePath}{_Attribute.toggleField}";
			var toggleProp = currentProp.serializedObject.FindProperty(togglePath);

			return toggleProp != null && toggleProp.propertyType == SerializedPropertyType.Boolean
			? toggleProp.boolValue
			: false;
		}

	}

}

#endif