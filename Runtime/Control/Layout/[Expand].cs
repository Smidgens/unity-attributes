// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draw structs expanded out
	/// </summary>
	public sealed class ExpandAttribute : __BaseControl
	{
		public ExpandAttribute(bool hideLabel = false)
		{
			this.hideLabel = hideLabel;
		}

		internal bool hideLabel { get; }
	}
}

/*
 * TODOS
 *	- support arrays
 */

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityObject = UnityEngine.Object;

	[CustomPropertyDrawer(typeof(ExpandAttribute))]
	internal sealed class _ExpandAttribute : __ControlDrawer<ExpandAttribute>
	{
		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			
			var total = Mathf.Max(0f, EditorGUIUtility.standardVerticalSpacing * (_fields.Count - 1));;

			if (!_Attribute.hideLabel)
			{
				total += EditorGUIUtility.standardVerticalSpacing;
				total += EditorGUIUtility.singleLineHeight;
			}

			foreach (var f in _fields)
			{
				total += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative(f.Name));
			}

			return total;
		}

		protected override void OnLabel(ref Rect pos, GUIContent l) { }

		protected override void OnField(in DrawContext ctx)
		{
			var tIndent = EditorGUI.indentLevel;
			
			var pos = ctx.position;

			if (!_Attribute.hideLabel)
			{
				EditorGUI.indentLevel++;
				var labelRect = pos.SliceTop(EditorGUIUtility.singleLineHeight);
				var label = !string.IsNullOrEmpty(_customLabel.text) ? _customLabel : ctx.label;
				EditorGUI.HandlePrefixLabel(ctx.position, labelRect, label);
				pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
			}

			foreach (var f in _fields)
			{
				var p = ctx.property.FindPropertyRelative(f.Name);
				var h = EditorGUI.GetPropertyHeight(p);
				var fRect = pos.SliceTop(h);

				
				fRect =	EditorGUI.PrefixLabel(fRect, new GUIContent(p.displayName));
				
				EditorGUI.indentLevel--;
				EditorGUI.PropertyField(fRect, p, GUIContent.none);
				EditorGUI.indentLevel++;
				
				
				pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
			}

			EditorGUI.indentLevel = tIndent;

		}

		protected override void OnInit()
		{
			_customLabel = new GUIContent(GetCustomLabel());
			_fields = fieldInfo.FieldType.GetInnermostType().FindInspectorFields<object>();
		}

		private GUIContent _customLabel;
		private IReadOnlyList<FieldInfo> _fields;



	}
}

#endif